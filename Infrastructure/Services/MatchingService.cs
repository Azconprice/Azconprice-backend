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

    public List<MatchResultDto> FindMatches(
        QueryRowDto query,
        IReadOnlyCollection<MasterRow> masterRows)
    {
        var qCan = _prep.Canon(query.Text);
        var qTokens = _prep.Tokenize(qCan).ToHashSet();
        var qNums = _num.Extract(query.Text);
        var qUnit = (query.Unit ?? "").Trim().ToLowerInvariant();

        var hits = new List<MatchResultDto>();

        foreach (var m in masterRows)
        {
            // 1. quick token overlap (excluding generic)
            if (!qTokens.Overlaps(m.TokenSet.Except(_vocab.Generic)))
                continue;

            // 2. coverage
            var cov = (double)qTokens.Intersect(m.TokenSet).Count() /
                      Math.Max(1, qTokens.Count);
            if (cov < MIN_COVER) continue;

            // 3. numerical guard & unit checks (same as python)
            var penal = 1.0;
            if (qNums.Count > 0)
            {
                var mNums = _num.Extract(m.OriginalText);
                if (mNums.Count > 0)
                {
                    bool anyExact = qNums.Any(q => mNums.Contains(q));
                    if (!anyExact) continue;               // size mismatch
                }
                else penal = 0.80;                        // python penalty
            }

            if (!string.IsNullOrEmpty(qUnit))
            {
                if (!m.Unit.Equals(qUnit, StringComparison.OrdinalIgnoreCase))
                    continue;
            }

            // 4. fuzzy score + critical penalty
            var score = Fuzz.TokenSetRatio(qCan, m.CanonText);
            if (_vocab.Critical.Any(c => qTokens.Contains(c) ^ m.TokenSet.Contains(c)))
                score = (int)(score * 0.70);

            score = (int)(score * penal);
            if (score < THRESHOLD) continue;

            hits.Add(new MatchResultDto(m.OriginalText, m.Price, m.Unit, score));
        }

        return hits.OrderByDescending(h => h.Score).ToList();
    }
}
