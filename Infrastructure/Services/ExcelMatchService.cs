// Infrastructure/Services/ExcelMatchService.cs
using Application.Models.DTOs;
using Application.Services;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace Infrastructure.Services;

public sealed class ExcelMatchService : IExcelMatchService, IDisposable
{
    private readonly IPreprocessingService _prep;
    private readonly IMatchingService _matcher;
    private readonly List<MasterRow> _master;

    public ExcelMatchService(IPreprocessingService prep,
                             INumericExtractor num,
                             IMatchingService matcher,
                             Vocab vocab,
                             string masterPath,
                             IConfiguration cfg)
    {
        _prep = prep;
        _matcher = matcher;
        _master = LoadMaster(masterPath, _prep);
    }

    // -------- IExcelMatchService -----------------------------------------
    public async Task<(byte[] Content, string FileName, string ContentType)>
        ProcessQueryExcelAsync(IFormFile queryFile)
    {
        using var mem = new MemoryStream();
        await queryFile.CopyToAsync(mem);
        mem.Position = 0;

        var wbIn = new XLWorkbook(mem);
        var wsIn = wbIn.Worksheet(1);
        var wbOut = new XLWorkbook();
        var wsOut = wbOut.AddWorksheet("Results");

        // header
        wsOut.Cell(1, 1).Value = "Query";
        wsOut.Cell(1, 2).Value = "Median";
        wsOut.Cell(1, 3).Value = "Average";
        wsOut.Cell(1, 4).Value = "Hits≥80";

        int idxOut = 2;
        foreach (var row in wsIn.RowsUsed().Skip(1))
        {
            var text = row.Cell(1).GetString();
            var flag = row.Cell(2).GetString();
            var unit = row.Cell(3).GetString();

            var hits = _matcher.FindMatches(new(text, flag, unit), _master);
            var priced = hits.Where(h => h.Score >= 80 && h.Price > 0).ToList();
            var prices = priced.Select(h => h.Price).ToList();

            double median = prices.Count == 0 ? 0
                             : prices.OrderBy(x => x)
                                     .ElementAt(prices.Count / 2);
            double average = prices.Count == 0 ? 0 : prices.Average();

            wsOut.Cell(idxOut, 1).Value = text;
            wsOut.Cell(idxOut, 2).Value = median;
            wsOut.Cell(idxOut, 3).Value = average;
            wsOut.Cell(idxOut, 4).Value = string.Join("; ",
                                 priced.Take(5).Select(p => $"{p.Price}/{p.Unit}"));
            idxOut++;
        }

        using var outMem = new MemoryStream();
        wbOut.SaveAs(outMem);
        return (outMem.ToArray(),
                "azcon_results.xlsx",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    // ---------------------------------------------------------------------
    private static List<MasterRow> LoadMaster(string path, IPreprocessingService prep)
    {
        var wb = new XLWorkbook(path);
        var ws = wb.Worksheet(1);

        var headerRow = ws.Row(1);
        var colByName = headerRow.CellsUsed()
                                 .ToDictionary(
                                     c => c.GetString().Trim().ToLowerInvariant(),
                                     c => c.Address.ColumnNumber);

        int colText = colByName["malların (işlərin və xidmətlərin) adı"];
        int colUnit = colByName["ölçü vahidi"];
        int colPrice = colByName["qiymət"];
        int colFlag = colByName["tip"];

        var list = new List<MasterRow>();
        var pricesByCanon = new Dictionary<string, List<double>>();

        foreach (var r in ws.RowsUsed().Skip(1))
        {
            var text = r.Cell(colText).GetString();
            var unit = r.Cell(colUnit).GetString().Trim().ToLowerInvariant();
            var flag = r.Cell(colFlag).GetString().Trim().ToLowerInvariant();
            var price = GetCellAsDouble(r.Cell(colPrice));
            if (string.IsNullOrWhiteSpace(text) || price <= 0) continue;

            var canon = prep.Canon(text);
            var tokens = prep.Tokenize(canon).ToHashSet();
            var material = prep.ExtractMaterial(text);

            if (!pricesByCanon.ContainsKey(canon))
                pricesByCanon[canon] = new List<double>();
            pricesByCanon[canon].Add(price);

            list.Add(new MasterRow(text, canon, flag, unit, price, tokens, material, 0));
        }

        // assign price medians
        foreach (var m in list)
        {
            var canon = m.CanonText;
            var priceList = pricesByCanon[canon];
            priceList.Sort();
            double median = priceList.Count switch
            {
                0 => 0,
                1 => priceList[0],
                _ => priceList.Count % 2 == 1
                    ? priceList[priceList.Count / 2]
                    : (priceList[priceList.Count / 2 - 1] + priceList[priceList.Count / 2]) / 2.0
            };
            m.PriceMedian = median;
        }

        return list;
    }


    private static double GetCellAsDouble(IXLCell cell)
    {
        if (cell.DataType == XLDataType.Number)
            return cell.GetDouble();

        var s = cell.GetString().Trim().Replace(',', '.');
        return double.TryParse(s,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out var d) ? d : 0.0;
    }
    public void Dispose() { }
}
