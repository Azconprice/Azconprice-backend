using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infrastructure.Helpers
{
    internal static class AzTextNormalizer
    {
        private static readonly string[] Suff = AzTextStaticData.Syn.Any() ? AzTextStaticData.Syn.Keys.ToArray() : Array.Empty<string>();
        private static readonly string[] SUF = { "lanması", "lənməsi", "lanma", "lənmə", "nması", "nməsi", "ması", "məsi", "ların", "lərin", "ları", "ləri", "lar", "lər" };
        private static readonly Dictionary<char, char> Tr = new() { { 'ğ', 'g' }, { 'ı', 'i' }, { 'ç', 'c' }, { 'ş', 's' }, { 'ö', 'o' }, { 'ü', 'u' }, { 'ə', 'e' } };
        private static string Transliterate(string s) => new(s.Select(c => Tr.TryGetValue(c, out var r) ? r : c).ToArray());

        public static string Canon(string txt)
        {
            if (string.IsNullOrWhiteSpace(txt)) return "";

            // (1) lower-case + transliterate first  🔁
            txt = txt.ToLowerInvariant();
            txt = Transliterate(txt);

            // (2) phrase (multi-word) synonyms AFTER transliteration
            foreach (var kv in AzTextStaticData.Syn.Where(kv => kv.Key.Contains(' ')))
                txt = txt.Replace(kv.Key, kv.Value);

            // (3) punctuation → space, collapse whitespace
            txt = Regex.Replace(txt, @"[^\w\s]", " ");
            txt = Regex.Replace(txt, @"\s+", " ").Trim();

            var outToks = new List<string>();
            foreach (var tok0 in txt.Split(' '))
            {
                if (AzTextStaticData.StopAz.Contains(tok0)) continue;   // now matches transliterated stop words
                var tok = tok0;
                foreach (var suf in SUF) if (tok.EndsWith(suf)) { tok = tok[..^suf.Length]; break; }
                tok = AzTextStaticData.Syn.TryGetValue(tok, out var map) ? map : tok;
                if (tok.Length > 0) outToks.Add(tok);
            }
            return string.Join(' ', outToks);
        }
        public static HashSet<string> TokenSet(string canon) => canon.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToHashSet();
        public static double Coverage(HashSet<string> a, HashSet<string> b) => a.Count == 0 ? 0 : a.Intersect(b).Count() / (double)a.Count;
    }


}
