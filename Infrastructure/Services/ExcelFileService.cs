using Application.Models;
using Application.Models.DTOs.Excel;
using Application.Models.DTOs.Pagination;
using Application.Repositories;
using Application.Services;
using AutoMapper;
using ClosedXML.Excel;
using Domain.Entities;
using Domain.Enums;
using FuzzySharp;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using RapidFuzz;           // ✅ RapidFuzz
// … other using-lines (AzTextNormalizer, etc.)





namespace Infrastructure.Services
{

    internal class ProductRow
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public string Type { get; set; } = string.Empty;
    }
    internal class QueryRow
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; }
        public decimal Qty { get; set; }
        public string Unit { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }

    public class ResultItem
    {
        public string QueryName { get; set; }
        public string Description { get; set; }
        public double Qty { get; set; }
        public string Unit { get; set; }
        public double AveragePrice { get; set; }

        public double TotalCost => Qty * AveragePrice;
    }

    internal class MatchedResult
    {
        public string QueryName { get; set; }
        public decimal Qty { get; set; }
        public string MatchedNames { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal MedianPrice { get; set; }
        public decimal TotalCost { get; set; }

        // NEW ↓
        public string Description { get; set; } = "";
        public string Unit { get; set; } = "";
    }

    public class ExcelFileService(
        IExcelFileRecordRepository repository,
        IBucketService bucketService,
        IMapper mapper,
           MasterFileOptions masterFileOptions,
           INumericExtractor numericExtractor) : IExcelFileService
    {
        private readonly string _masterFilePath = masterFileOptions.MasterPath;
        private readonly IExcelFileRecordRepository _repository = repository;
        private readonly IBucketService _bucketService = bucketService;
        private readonly IMapper _mapper = mapper;
        private readonly INumericExtractor _numericExtractor = numericExtractor;

        public async Task<PaginatedResult<ExcelFileDTO>> GetExcelFilesAsync(PaginationRequest request)
        {
            var query = _repository.Query()
                .OrderByDescending(x => x.UploadedAt);

            var totalCount = await query.CountAsync();

            var files = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var result = new List<ExcelFileDTO>();
            foreach (var file in files)
            {
                var dto = _mapper.Map<ExcelFileDTO>(file);
                dto.Url = await _bucketService.GetSignedUrlAsync(file.FilePath);
                result.Add(dto);
            }

            return new PaginatedResult<ExcelFileDTO>
            {
                Items = result,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<int> GetExcelFileCountAsync()
        {
            return await _repository.GetTotalCountAsync();
        }

        public async Task<ExcelFileDTO> UploadExcelAsync(IFormFile file, string firstName, string lastName, string email, string userId)
        {
            var path = await _bucketService.UploadExcelAsync(file, firstName, lastName, email, userId);

            var record = new ExcelFileRecord
            {
                Id = Guid.NewGuid(),
                FilePath = path,
                FileName = Path.GetFileName(path),
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                UserId = userId,
                UploadedAt = DateTime.UtcNow,
                Status = ExcelFileStatus.Pending
            };

            await _repository.AddAsync(record);
            await _repository.SaveChangesAsync();

            var dto = _mapper.Map<ExcelFileDTO>(record);
            dto.Url = await _bucketService.GetSignedUrlAsync(record.FilePath);

            return dto;
        }

        public async Task<PaginatedResult<ExcelFileDTO>> GetExcelFilesByUserAsync(string userId, PaginationRequest request)
        {
            var query = _repository.Query()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.UploadedAt);

            var totalCount = await query.CountAsync();
            var files = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var result = new List<ExcelFileDTO>();
            foreach (var file in files)
            {
                var dto = _mapper.Map<ExcelFileDTO>(file);
                dto.Url = await _bucketService.GetSignedUrlAsync(file.FilePath);
                result.Add(dto);
            }

            return new PaginatedResult<ExcelFileDTO>
            {
                Items = result,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }

        public FileContentResult ProcessQueryExcelAsync(
    IFormFile queryFile,
    string? userId = null,
    bool isSimple = false)
        {
            if (queryFile == null || queryFile.Length == 0)
                throw new ArgumentException("Excel file is required.", nameof(queryFile));

            const int MIN_SCORE = 65;
            const int PRICE_SCORE = 82;
            const double MIN_COVER = 0.50;

            if (!File.Exists(_masterFilePath))
                throw new FileNotFoundException("Master file not found", _masterFilePath);

            var (master, ix) = LoadMaster(_masterFilePath);
            var queries = LoadQueries(queryFile);

            var results = new List<MatchedResult>();
            var matchRows = new List<(string Q, string M, int S, decimal? P, string U, string T, string D)>();

            foreach (var q in queries)
            {
                var qCan = AzTextNormalizer.Canon(q.Name);
                var qTok = AzTextNormalizer.TokenSet(qCan);
                var qNums = _numericExtractor.Extract(q.Name);
                bool hasQn = qNums.Count > 0;

                var qMaterial = AzTextNormalizer.ExtractMaterial(qCan);

                IEnumerable<ProductRow> cand = master;

                if (!string.IsNullOrEmpty(qMaterial))
                {
                    cand = master.Where(r => AzTextNormalizer.ExtractMaterial(r.Name) == qMaterial);
                }

                if (q.Type is "product" or "service" or "mix")
                    cand = cand.Where(m => m.Type == q.Type);
                if (!string.IsNullOrWhiteSpace(q.Unit))
                    cand = cand.Where(m => m.Unit == q.Unit);

                var hits = new List<(ProductRow Row, int Score)>();

                foreach (var m in cand)
                {
                    var (mCan, mTok) = ix[m];

                    if (!qTok.Intersect(mTok.Except(AzTextStaticData.Generic)).Any())
                        continue;

                    if (AzTextNormalizer.Coverage(qTok, mTok) < MIN_COVER)
                        continue;

                    double numPenalty = 1.0;
                    if (hasQn)
                    {
                        var mNums = _numericExtractor.Extract(m.Name);
                        if (mNums.Count == 0)
                        {
                            numPenalty = 0.80;
                        }
                        else
                        {
                            bool exact = qNums.Any(qp =>
                                mNums.Any(mp =>
                                    Math.Abs(mp.Number - qp.Number) < 1e-6 &&
                                    mp.Unit == qp.Unit));
                            if (!exact) continue;
                        }
                    }

                    bool critMismatch = AzTextStaticData.Critical
                        .Any(c => qTok.Contains(c) ^ mTok.Contains(c));
                    if (critMismatch) continue;

                    double raw = Fuzz.TokenSetRatio(qCan, mCan) * numPenalty;
                    int score = (int)raw;
                    if (score < MIN_SCORE) continue;

                    hits.Add((m, score));
                }

                var strong = hits.Where(h => h.Score >= PRICE_SCORE && h.Row.Price.HasValue)
                                 .OrderBy(h => h.Row.Price!.Value)
                                 .ToList();

                foreach (var h in strong)
                    matchRows.Add((q.Name, h.Row.Name, h.Score,
                                   h.Row.Price, h.Row.Unit, h.Row.Type, h.Row.Description ?? ""));

                var prices = strong.Select(h => h.Row.Price!.Value).OrderBy(p => p).ToList();

                decimal avg = prices.Count == 0 ? 0 : Math.Round(prices.Average(), 2);
                decimal med = prices.Count switch
                {
                    0 => 0,
                    1 => prices[0],
                    _ => prices.Count % 2 == 1
                           ? prices[prices.Count / 2]
                           : Math.Round((prices[prices.Count / 2 - 1] + prices[prices.Count / 2]) / 2m, 2)
                };
                decimal tot = Math.Round(avg * q.Qty, 2);

                results.Add(new MatchedResult
                {
                    QueryName = q.Name,
                    Qty = q.Qty,
                    AveragePrice = avg,
                    MedianPrice = med,
                    TotalCost = tot
                });
            }

            using var wbOut = new XLWorkbook();
            if (isSimple)
            {
                var simple = wbOut.Worksheets.Add("Results-Simple");
                simple.Cell(1, 1).SetValue("Mallarin (işlərin və xidmətlərin) adı");
                simple.Cell(1, 2).SetValue("Ətraflı təsviri");
                simple.Cell(1, 3).SetValue("Həcmi / Miqdarı");
                simple.Cell(1, 4).SetValue("Ölçü vahidi");
                simple.Cell(1, 5).SetValue("Qiymət");
                simple.Cell(1, 6).SetValue("Cəm (₼)");

                for (int i = 0; i < results.Count; i++)
                {
                    var r = results[i];
                    var q = queries[i];
                    int row = i + 2;
                    simple.Cell(row, 1).Value = r.QueryName;
                    simple.Cell(row, 2).Value = q.Description ?? "";
                    simple.Cell(row, 3).Value = q.Qty;
                    simple.Cell(row, 4).Value = q.Unit ?? "";
                    simple.Cell(row, 5).Value = r.AveragePrice;
                    simple.Cell(row, 6).Value = r.TotalCost;
                }

                int footer = results.Count + 2;
                simple.Cell(footer, 5).Value = "Ümumi Toplam:";
                simple.Cell(footer, 6).FormulaA1 = $"=SUM(F2:F{results.Count + 1})";
                simple.Cell(footer, 6).Style.NumberFormat.Format = "#,#0.00";
                simple.Columns().AdjustToContents();
            }
            else
            {
                var rs = wbOut.Worksheets.Add("Results");
                rs.Cell(1, 1).SetValue("Sorğu Adı");
                rs.Cell(1, 2).SetValue("Miqdar");
                rs.Cell(1, 3).SetValue("Uyğun Gələn Məhsullar");
                rs.Cell(1, 4).SetValue("Orta Qiymət (₼)");
                rs.Cell(1, 5).SetValue("Median Qiymət (₼)");
                rs.Cell(1, 6).SetValue("Cəm (₼)");

                for (int i = 0; i < results.Count; i++)
                {
                    var r = results[i];
                    rs.Cell(i + 2, 1).Value = r.QueryName;
                    rs.Cell(i + 2, 2).Value = r.Qty;
                    rs.Cell(i + 2, 3).Value = string.Join(" | ", matchRows.Where(m => m.Q == r.QueryName).Select(m => $"{m.M} – {m.P:0.##} ₼/{m.U} ({m.S}%)"));
                    rs.Cell(i + 2, 4).Value = r.AveragePrice;
                    rs.Cell(i + 2, 5).Value = r.MedianPrice;
                    rs.Cell(i + 2, 6).Value = r.TotalCost;
                }

                rs.Cell(results.Count + 3, 5).Value = "Ümumi Toplam:";
                rs.Cell(results.Count + 3, 6).FormulaA1 = $"=SUM(F2:F{results.Count + 1})";
                rs.Cell(results.Count + 3, 6).Style.NumberFormat.Format = "#,#0.00";

                var ms = wbOut.Worksheets.Add("Matches");
                ms.Cell(1, 1).SetValue("Sorğu Adı");
                ms.Cell(1, 2).SetValue("Uygun Ad");
                ms.Cell(1, 3).SetValue("Score");
                ms.Cell(1, 4).SetValue("Qiymət");
                ms.Cell(1, 5).SetValue("Ölçü vahidi");
                ms.Cell(1, 6).SetValue("Tip");
                ms.Cell(1, 7).SetValue("Təsvir");

                for (int i = 0; i < matchRows.Count; i++)
                {
                    var (Q, M, S, P, U, T, D) = matchRows[i];
                    ms.Cell(i + 2, 1).Value = Q;
                    ms.Cell(i + 2, 2).Value = M;
                    ms.Cell(i + 2, 3).Value = S;
                    ms.Cell(i + 2, 4).Value = P;
                    ms.Cell(i + 2, 5).Value = U;
                    ms.Cell(i + 2, 6).Value = T;
                    ms.Cell(i + 2, 7).Value = D;
                }
            }

            using var mem = new MemoryStream();
            wbOut.SaveAs(mem);
            mem.Position = 0;

            return new FileContentResult(mem.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = "processed_results.xlsx"
            };
        }

        /*──────────────────────── HELPERS ─────────────────────────────*/

        private static (List<ProductRow>, Dictionary<ProductRow, (string, HashSet<string>)>)
            LoadMaster(string path)
        {
            var list = new List<ProductRow>();
            var ix = new Dictionary<ProductRow, (string, HashSet<string>)>();

            using var wb = new XLWorkbook(path);
            var ws = wb.Worksheet(1);
            var hdr = ws.Row(1).CellsUsed()
                        .ToDictionary(c => c.GetString().Trim().ToLowerInvariant(),
                                      c => c.Address.ColumnNumber);

            int colName = hdr["malların (işlərin və xidmətlərin) adı"];
            int colUnit = hdr["ölçü vahidi"];
            int colPrice = hdr["qiymət"];
            int colFlag = hdr["tip"];
            int colDesc = hdr.TryGetValue("ətraflı təsviri", out var d) ? d : 0;

            foreach (var r in ws.RowsUsed().Skip(1))
            {
                var name = r.Cell(colName).GetString();
                if (string.IsNullOrWhiteSpace(name)) continue;

                var row = new ProductRow
                {
                    Name = name,
                    Description = colDesc == 0 ? "" : r.Cell(colDesc).GetString(),
                    Unit = r.Cell(colUnit).GetString().Trim().ToLowerInvariant(),
                    Price = ParsePrice(r.Cell(colPrice)),
                    Type = r.Cell(colFlag).GetString().Trim().ToLowerInvariant()
                };
                list.Add(row);

                var can = AzTextNormalizer.Canon(name);
                ix[row] = (can, AzTextNormalizer.TokenSet(can));
            }

            return (list, ix);
        }
        private static List<QueryRow> LoadQueries(IFormFile file)
        {
            using var wb = new XLWorkbook(file.OpenReadStream());
            var ws = wb.Worksheet(1);
            var hdr = ws.Row(1).CellsUsed()
                        .ToDictionary(c => c.GetString().Trim().ToLowerInvariant(),
                                      c => c.Address.ColumnNumber);

            int colName = hdr["malların (işlərin və xidmətlərin) adı"];
            int colUnit = hdr.TryGetValue("ölçü vahidi", out var cu) ? cu : 0;
            int colFlag = hdr.TryGetValue("tip", out var cf) ? cf : 0;
            int colDesc = hdr.TryGetValue("ətraflı təsviri", out var cd) ? cd : 0;
            int colQty = hdr.TryGetValue("həcmi / miqdarı", out var cq) ? cq : 0;

            var list = new List<QueryRow>();
            foreach (var r in ws.RowsUsed().Skip(1))
            {
                var name = r.Cell(colName).GetString();
                if (string.IsNullOrWhiteSpace(name)) continue;

                decimal qty = 0;
                if (colQty != 0)
                {
                    var raw = r.Cell(colQty).GetString().Replace(',', '.');
                    decimal.TryParse(raw, NumberStyles.Any,
                        CultureInfo.InvariantCulture, out qty);
                }

                list.Add(new QueryRow
                {
                    Name = name,
                    Description = colDesc == 0 ? "" : r.Cell(colDesc).GetString(),
                    Qty = qty,
                    Unit = colUnit == 0 ? "" : r.Cell(colUnit).GetString().Trim().ToLowerInvariant(),
                    Type = colFlag == 0 ? "" : r.Cell(colFlag).GetString().Trim().ToLowerInvariant()
                });
            }
            return list;
        }

        private static decimal? ParsePrice(IXLCell cell)
        {
            if (cell.DataType == XLDataType.Number)
                return (decimal)cell.GetDouble();

            var txt = cell.GetString()
                        .Replace("\u00A0", "") // nbsp
                        .Replace(" ", "")      // thin space
                        .Replace(",", ".");
            return decimal.TryParse(txt, NumberStyles.Any,
                CultureInfo.InvariantCulture, out var d) ? d : (decimal?)null;
        }

    }
}