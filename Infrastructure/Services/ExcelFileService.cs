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
           MasterFileOptions masterFileOptions) : IExcelFileService
    {
        private readonly string _masterFilePath = masterFileOptions.MasterPath;
        private readonly IExcelFileRecordRepository _repository = repository;
        private readonly IBucketService _bucketService = bucketService;
        private readonly IMapper _mapper = mapper;

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

        public FileContentResult ProcessQueryExcelAsync(IFormFile queryFile, string? userId = null)
        {
            if (queryFile == null || queryFile.Length == 0)
                throw new ArgumentException("Excel file is required.", nameof(queryFile));

            const int MIN_SCORE = 65;
            const int PRICE_SCORE = 80;
            const double MIN_COVER = 0.50;

            // 1️⃣ LOAD MASTER
            if (!File.Exists(_masterFilePath))
                throw new FileNotFoundException("Master file not found", _masterFilePath);

            var masterRows = new List<ProductRow>();
            var canonIx = new Dictionary<ProductRow, (string canon, HashSet<string> tok)>();

            using (var wb = new XLWorkbook(_masterFilePath))
            {
                var ws = wb.Worksheet(1);
                foreach (var r in ws.RowsUsed().Skip(1))
                {
                    var name = r.Cell(1).GetString();
                    if (string.IsNullOrWhiteSpace(name)) continue;

                    var row = new ProductRow
                    {
                        Name = name,
                        Description = r.Cell(2).GetString(),
                        Quantity = r.Cell(3).GetString(),
                        Unit = r.Cell(4).GetString().Trim().ToLowerInvariant(),
                        Price = decimal.TryParse(r.Cell(5).GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var p) ? p : null,
                        Type = r.Cell(6).GetString().Trim().ToLowerInvariant()
                    };
                    masterRows.Add(row);
                    var canon = AzTextNormalizer.Canon(name);
                    canonIx[row] = (canon, AzTextNormalizer.TokenSet(canon));
                }
            }

            masterRows = masterRows
                .GroupBy(m => new { m.Name, m.Type, m.Unit, m.Price })
                .Select(g => g.First()).ToList();

            // 2️⃣ LOAD QUERY FILE
            var queries = new List<QueryRow>();
            using (var wbQ = new XLWorkbook(queryFile.OpenReadStream()))
            {
                var ws = wbQ.Worksheet(1);
                foreach (var r in ws.RowsUsed().Skip(1))
                {
                    var qName = r.Cell(1).GetString();
                    if (string.IsNullOrWhiteSpace(qName)) continue;
                    decimal qty = 0;
                    var qtyStr = r.Cell(3).GetString();
                    if (decimal.TryParse(qtyStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var qTmp)) qty = qTmp;
                    else if (r.Cell(3).TryGetValue(out double qNum)) qty = (decimal)qNum;

                    queries.Add(new QueryRow
                    {
                        Name = qName,
                        Description = r.Cell(2).GetString(),
                        Qty = qty,
                        Unit = r.Cell(4).GetString().Trim().ToLowerInvariant(),
                        Type = r.Cell(6).GetString().Trim().ToLowerInvariant()
                    });
                }
            }

            // 3️⃣ MATCH & CALCULATE
            var results = new List<MatchedResult>();

            foreach (var q in queries)
            {
                var qCanon = AzTextNormalizer.Canon(q.Name);
                var qTok = AzTextNormalizer.TokenSet(qCanon);
                var contQ = qTok.Except(AzTextStaticData.Generic).ToHashSet();

                IEnumerable<ProductRow> candidates = masterRows;
                if (q.Type == "product" || q.Type == "service" || q.Type == "mix")
                    candidates = candidates.Where(m => m.Type == q.Type);
                if (!string.IsNullOrWhiteSpace(q.Unit))
                    candidates = candidates.Where(m => m.Unit == q.Unit);

                var hits = new List<(ProductRow Row, int Score)>();
                foreach (var m in candidates)
                {
                    var (mCanon, mTok) = canonIx[m];
                    if (!contQ.Overlaps(mTok.Except(AzTextStaticData.Generic))) continue;

                    double raw = Fuzz.TokenSetRatio(qCanon, mCanon);
                    if (AzTextStaticData.Critical.Any(c => qTok.Contains(c) ^ mTok.Contains(c))) raw *= 0.70;
                    int score = (int)raw;
                    if (score < MIN_SCORE || AzTextNormalizer.Coverage(qTok, mTok) < MIN_COVER) continue;
                    hits.Add((m, score));
                }

                var strong = hits.Where(h => h.Score >= PRICE_SCORE && h.Row.Price.HasValue)
                                 .OrderBy(h => h.Row.Price.Value).ToList();

                if (!strong.Any())
                {
                    results.Add(new MatchedResult
                    {
                        QueryName = q.Name,
                        Qty = q.Qty,
                        AveragePrice = 0,
                        MedianPrice = 0,
                        TotalCost = 0
                    });
                    continue;
                }

                decimal avg = Math.Round(strong.Average(h => h.Row.Price!.Value), 2);
                decimal med = strong.Count % 2 == 1 ? strong[strong.Count / 2].Row.Price!.Value :
                               Math.Round((strong[strong.Count / 2 - 1].Row.Price!.Value + strong[strong.Count / 2].Row.Price!.Value) / 2m, 2);
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

            // 4️⃣ BUILD SIMPLE SHEET ONLY
            using var wbOut = new XLWorkbook();
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

            // 5️⃣ RETURN FILE
            using var mem = new MemoryStream();
            wbOut.SaveAs(mem); mem.Position = 0;

            return new FileContentResult(mem.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = "processed_results.xlsx"
            };
        }
    }
}