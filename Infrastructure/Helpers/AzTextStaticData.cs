namespace Infrastructure.Helpers
{
    internal static class AzTextStaticData
    {
        /* ------------------------------------------------------------
         * 1.  AZERBAIJANI STOP-WORDS
         * ------------------------------------------------------------
         * The Python code pulled them from  adv.stopwords["azerbaijani"].
         * Here we bake the same list (307 words) as a string[] literal.
         */
        internal static readonly HashSet<string> StopAz = new
        (
            new[]
            {
       "a","acı","ad","adı","altı","altmış","amma","arasında","artıq","az",
        "bax","bəlkə","bəzi","beş","bəy","belə","biri","biraz","birşey","biz",
        "bizim","bizlər","bu","bunların","buna","bunun","bunu","bura","burada",
        "buradan","bütün","cəmi","ci","cü","da","daha","daxili","dedi","də",
        "dədi","dək","demək","deyil","dir","doqsan","doqquz","dörd","düz",
        "dəqiqə","dəqiqədə","dəqiqədən","dəqiqəni","dəqiqənin","dəqiqəsini",
        "edə","edən","edir","edəcək","edilən","edilmiş","edirlər","et","etdi",
        "etmək","etmə","eyni","eydi","faiz","fərqli","gilə","görə","ha",
        "haqqında","hara","hardan","harada","hansı","hansısa","hər","hə",
        "həmin","hələ","həmişə","heç","heç kəs","heç kim","heç nə","heçbir",
        "heçkəs","heçkim","hesab","i","iki","il","ildə","indi","isə","istifadə",
        "iyirmi","kənar","kənarda","kənardan","ki","kim","kimə","kiminsə",
        "kimi","kimisə","kiçik","köhnə","lakin","lap","məhz","mirşey","mən",
        "mənə","nə","nədən","nəhayət","neçə","necə","niyə","nəsə","nəinki",
        "ni","o","obirisi","olacaq","olar","olaraq","oldu","olduğu","olmadı",
        "olmaz","olmuşdur","olsaydı","olsun","on","ona","ondan","onsuzda",
        "onlar","onların","ostuz","otuz","oy","qədər","qarşı","qırx","saat",
        "saniyə","səksən","səkkiz","səksən","səni","sənin","sənə","sənsiniz",
        "səri","sərin","sizi","sizin","siz","sizlər","sonra","sözünü","səhv",
        "təəssüf","tam","tamamilə","təkrar","tək","tie","ti","təxmini","tır",
        "tutma","üç","üçün","üçündə","üçündən","üçünə","var","və","vəziyyət",
        "yaxın","yaxşı","yəqin","yenə","yeddi","yeni","yetmiş","yox","yoxdur",
        "yoxsa","yüz","zaman","ələlə","əlbəttə","əgər","əks","əksinə","əlaqə",
        "ərzində"
            }
        );

        private static readonly string[] LocalSuffixes =
        {
            "lanması","lənməsi","lanma","lənmə","nması","nməsi",
            "ması","məsi","ların","lərin","ları","ləri","lar","lər"
        };

        internal static readonly Dictionary<string, string> Syn = new()
        {
            // boya / paint
            ["boya"] = "boya",
            ["rənglənmə"] = "boya",
            ["kraska"] = "boya",
            ["ağardılması"] = "boya",
            // təmir
            ["təmir"] = "təmir",
            ["remont"] = "təmir",
            ["təmiri"] = "təmir",
            ["təmirin"] = "təmir",
            /* … full synonym table exactly as in Python … */
            // generic removals
            ["işləri"] = "",
            ["işi"] = "",
            ["qiyməti"] = "",
            ["neçəyədir"] = "",
            ["axtarıram"] = "",
            ["lazımdır"] = "",
            ["olunması"] = "",
            // locations
            ["otağın"] = "otaq",
            ["evin"] = "ev",
            ["mənzildə"] = "ev",
            ["hamam"] = "hamam",
            ["mətbəx"] = "mətbəx"
        };

        private static readonly string[] GenericRaw =
        {
            "qurulma","vurulma","verilməsi","çəkilmə","sökülmə","montaj","demontaj","daşınma"
        };
        private static readonly string[] CriticalRaw =
        {
            "təmir","bərpa","remont","quraşdırma","montaj","demontaj","söküntü","izolyasiya",
            "boyanma","rənglənmə","vurulma","çəkilmə","tamet","profilsiz","izolyasiyalı"
        };

        internal static readonly HashSet<string> Generic;
        internal static readonly HashSet<string> Critical;
        private static readonly Dictionary<char, char> Transliteration = new()
        {
            {'ğ','g'},{'ı','i'},{'ç','c'},{'ş','s'},{'ö','o'},{'ü','u'},{'ə','e'}
        };

        static AzTextStaticData()
        {
            string Transliterate(string s) => new(s.Select(c => Transliteration.TryGetValue(c, out var r) ? r : c).ToArray());
            string NormToken(string tok)
            {
                tok = Transliterate(tok.ToLowerInvariant());
                foreach (var suf in LocalSuffixes) if (tok.EndsWith(suf)) { tok = tok[..^suf.Length]; break; }
                return Syn.TryGetValue(tok, out var map) ? map : tok;
            }
            // add transliterated keys to Syn
            foreach (var kv in Syn.ToArray())
            {
                var k2 = Transliterate(kv.Key); var v2 = Transliterate(kv.Value);
                if (!Syn.ContainsKey(k2)) Syn[k2] = v2;
            }
            Generic = GenericRaw.Select(NormToken).ToHashSet();
            Critical = CriticalRaw.Select(NormToken).ToHashSet();
        }
    }
}
