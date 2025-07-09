// Infrastructure/Services/PreprocessingService.cs
using Application.Services;
using System.Text;
using System.Text.RegularExpressions;

namespace Infrastructure.Services;

public sealed class PreprocessingService : IPreprocessingService
{
    private readonly Vocab _vocab;
    private readonly HashSet<string> _stopAz;
    private readonly List<string> _suffixes;

    private static readonly Regex _punct = new("[^\\w\\s]", RegexOptions.Compiled);
    private static readonly Regex _ws = new("\\s{2,}", RegexOptions.Compiled);
    private static readonly Regex _split = new("\\s+", RegexOptions.Compiled);

    private static readonly Dictionary<char, char> _translit =
        new() { ['ğ'] = 'g', ['ı'] = 'i', ['ç'] = 'c', ['ş'] = 's', ['ö'] = 'o', ['ü'] = 'u', ['ə'] = 'e' };

    public PreprocessingService(Vocab vocab)
    {
        _vocab = vocab;

        // use Advertools AZ stop-list like python (hard-coded small subset here)
        _stopAz = new HashSet<string>(new[]{
            "və","üçün","ilə","bu","bir","ki","həm","daha","olan"
        });

        // reuse suffix list from python
        _suffixes = new(){
            "lanması","lənməsi","lanma","lənmə","nması","nməsi",
            "ması","məsi","ların","lərin","ları","ləri","lar","lər"
        };
        _suffixes.Sort((a, b) => b.Length.CompareTo(a.Length));
    }

    // -----------------------------------------------------------
    public string Canon(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return "";

        // 1. lower + translit
        var txt = LowerTranslit(input);

        // 2. phrase synonyms
        foreach (var kv in _vocab.Synonyms.Where(x => x.Key.Contains(' ')))
            txt = txt.Replace(kv.Key, kv.Value, StringComparison.Ordinal);

        // 3. strip punctuation
        txt = _punct.Replace(txt, " ");
        txt = _ws.Replace(txt, " ").Trim();

        // 4-6 per-token
        var tokens = new List<string>();
        foreach (var tok in _split.Split(txt))
        {
            if (_stopAz.Contains(tok)) continue;
            var baseTok = StripSuffix(tok);
            var norm = _vocab.Synonyms.TryGetValue(baseTok, out var repl)
                          ? repl : baseTok;
            if (norm.Length > 0) tokens.Add(norm);
        }
        return string.Join(' ', tokens);
    }

    public List<string> Tokenize(string canonText) => _split.Split(canonText)
                                                            .Where(t => t.Length > 0)
                                                            .ToList();
    // ----------------------------------------------------------- helpers
    private string StripSuffix(string tok)
    {
        foreach (var suf in _suffixes)
            if (tok.EndsWith(suf, StringComparison.Ordinal))
                return tok[..^suf.Length];
        return tok;
    }

    private static string LowerTranslit(string src)
    {
        var sb = new StringBuilder(src.Length);
        foreach (var ch in src)
        {
            var lc = char.ToLowerInvariant(ch);
            sb.Append(_translit.TryGetValue(lc, out var r) ? r : lc);
        }
        return sb.ToString();
    }
}
