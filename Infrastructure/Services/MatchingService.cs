// Infrastructure/Services/MatchingService.cs
using Application.Models;
using Application.Models.DTOs;
using Application.Services;
using FuzzySharp;

namespace Infrastructure.Services;

public sealed class MatchingService : IMatchingService
{
    private const int THRESHOLD = 65;   // :contentReference[oaicite:1]{index=1}
    private const double MIN_COVER = 0.50; // :contentReference[oaicite:2]{index=2}
    private const int PRICE_MIN = 80;   // Python PRICE_AVG_MIN_SCORE

    private readonly IPreprocessingService _prep;
    private readonly INumericExtractor _num;
    private readonly Vocab _vocab;

    public MatchingService(IPreprocessingService prep,
                           INumericExtractor num,
                           Vocab vocab)
    {
        _prep = prep; _num = num; _vocab = vocab;
    }

    public List<MatchResultDto> FindMatches(QueryRowDto query, IReadOnlyCollection<MasterRow> masterRows)
    {
        var qCan = _prep.Canon(query.Text);
        var qTokens = _prep.Tokenize(qCan).ToHashSet();
        var qNums = _num.Extract(query.Text);
        var qUnit = (query.Unit ?? "").Trim().ToLowerInvariant();
        var qMaterial = _prep.ExtractMaterial(query.Text);
        var qFlag = (query.Flag ?? "").Trim().ToLowerInvariant();

        // Material-based filtering
        IEnumerable<MasterRow> candidates;
        if (!string.IsNullOrWhiteSpace(qMaterial))
        {
            candidates = masterRows.Where(m => m.Material == qMaterial);
        }
        else
        {
            var grouped = masterRows
                .Where(m => m.Material != null)
                .GroupBy(m => m.Material!)
                .Select(g => new {
                    Material = g.Key,
                    Median = g.Select(x => x.PriceMedian).DefaultIfEmpty().Average()
                })
                .OrderBy(x => x.Median)
                .FirstOrDefault();

            if (grouped != null)
                candidates = masterRows.Where(m => m.Material == grouped.Material);
            else
                candidates = masterRows;
        }

        // Type/unit filtering
        if (!string.IsNullOrWhiteSpace(qFlag))
            candidates = candidates.Where(m => m.Flag == qFlag);
        if (!string.IsNullOrWhiteSpace(qUnit))
            candidates = candidates.Where(m => m.Unit == qUnit);

        var hits = new List<MatchResultDto>();
        foreach (var m in candidates)
        {
            if (!qTokens.Overlaps(m.TokenSet.Except(_vocab.Generic)))
                continue;

            var cov = (double)qTokens.Intersect(m.TokenSet).Count() / Math.Max(1, qTokens.Count);
            if (cov < 0.5) continue;

            double penal = 1.0;
            if (qNums.Count > 0)
            {
                var mNums = _num.Extract(m.OriginalText);
                if (mNums.Count > 0)
                {
                    bool anyExact = qNums.Any(q => mNums.Contains(q));
                    if (!anyExact) continue;
                }
                else penal = 0.80;
            }

            if (_vocab.Critical.Any(c => qTokens.Contains(c) ^ m.TokenSet.Contains(c)))
                continue;

            var score = Fuzz.TokenSetRatio(qCan, m.CanonText);
            score = (int)(score * penal);
            if (score < 80) continue;

            hits.Add(new MatchResultDto(m.OriginalText, m.Price, m.Unit, score));
        }

        return hits.OrderByDescending(h => h.Score).ToList();
    }
}
