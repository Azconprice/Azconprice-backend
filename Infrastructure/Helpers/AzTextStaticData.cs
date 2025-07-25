using Application.Models;

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
       "a","ad","altı","altmış","amma","arasında","artıq","ay","az","bax",
  "belə","beş","bilər","bir","biraz","biri","birşey","biz","bizim","bizlər",
  "bu","buna","bundan","bunların","bunu","bunun","buradan","bütün","bəli","bəlkə",
  "bəy","bəzi","bəzən","ci","cu","cü","cı","da","daha","dedi",
  "deyil","dir","doqquz","doqsan","dörd","düz","də","dək","dən","dəqiqə",
  "edir","edən","elə","et","etdi","etmə","etmək","faiz","gilə","görə",
  "ha","haqqında","harada","heç","hə","həm","həmin","həmişə","hər","idi",
  "iki","il","ildə","ilk","ilə","in","indi","istifadə","isə","iyirmi",
  "ki","kim","kimi","kimə","lakin","lap","mirşey","məhz","mən","mənə",
  "niyə","nə","nəhayət","o","obirisi","of","olan","olar","olaraq","oldu",
  "olduğu","olmadı","olmaz","olmuşdur","olsun","olur","on","ona","ondan","onlar",
  "onlardan","onların","onsuzda","onu","onun","oradan","otuz","qarşı","qırx","qədər",
  "saat","sadəcə","saniyə","siz","sizin","sizlər","sonra","səhv","səkkiz","səksən",
  "sən","sənin","sənə","təəssüf","var","və","xan","xanım","xeyr","ya",
  "yalnız","yaxşı","yeddi","yenə","yetmiş","yox","yoxdur","yoxsa","yüz","yəni",
  "zaman","çox","çünki","öz","özü","ü","üç","üçün","ı","ə",
  "əgər","əlbəttə","əlli","ən","əslində"
            }
        );

        private static readonly string[] LocalSuffixes =
        {
            "lanması","lənməsi","lanma","lənmə","nması","nməsi",
            "ması","məsi","ların","lərin","ları","ləri","lar","lər"
        };

        internal static readonly Dictionary<string, string> Syn = new()
        {
            ["boya"] = "boya",
            ["kraska"] = "boya",
            ["ağardılması"] = "boya",
            ["təmir"] = "təmir",
            ["remont"] = "təmir",
            ["təmiri"] = "təmir",
            ["təmirin"] = "təmir",
            ["təmırı"] = "təmir",
            ["təmırın"] = "təmir",
            ["bərpa"] = "təmir",
            ["alçipan"] = "alçipan",
            ["alcipan"] = "alçipan",
            ["alcıpanl"] = "alçipan",
            ["gipsokarton"] = "alçipan",
            ["alçıpanla"] = "alçipan",
            ["divar"] = "divar",
            ["divarlar"] = "divar",
            ["divarın"] = "divar",
            ["divarların"] = "divar",
            ["dıvar"] = "divar",
            ["arakəsmə"] = "divar",
            ["divarlarının"] = "divar",
            ["kafel"] = "kafel",
            ["metlax"] = "kafel",
            ["kafe"] = "kafel",
            ["kafelinin"] = "kafel",
            ["metlaxın"] = "kafel",
            ["döşəmə"] = "döşəmə",
            ["döşənmə"] = "döşəmə",
            ["döşəməsinin"] = "döşəmə",
            ["laminat"] = "döşəmə",
            ["parket"] = "döşəmə",
            ["pol"] = "döşəmə",
            ["laminatdan"] = "döşəmə",
            ["quraşdırma"] = "quraşdırma",
            ["quraşdırılması"] = "quraşdırma",
            ["montaj"] = "quraşdırma",
            ["montaji"] = "quraşdırma",
            ["qurulması"] = "quraşdırma",
            ["çəkilmə"] = "quraşdırma",
            ["çəkilməsi"] = "quraşdırma",
            ["vurulma"] = "quraşdırma",
            ["yığılma"] = "quraşdırma",
            ["yığıl"] = "quraşdırma",
            ["qurul"] = "quraşdırma",
            ["qoyulması"] = "quraşdırma",
            ["sökülmə"] = "sökülmə",
            ["sökülməsi"] = "sökülmə",
            ["söküntü"] = "sökülmə",
            ["söküntüsü"] = "sökülmə",
            ["təmizlənmə"] = "təmizlənmə",
            ["təmizlənməsi"] = "təmizlənmə",
            ["daşınması"] = "daşınma",
            ["daşınma"] = "daşınma",
            ["izolyasiya"] = "izolyasiya",
            ["hidroizolyasiya"] = "izolyasiya",
            ["izolyasiyalı"] = "izolyasiya",
            ["suvaq"] = "suvaq",
            ["suvağı"] = "suvaq",
            ["şpatlyovka"] = "suvaq",
            ["beton"] = "beton",
            ["betonlanması"] = "beton",
            ["tavan"] = "tavan",
            ["tavanın"] = "tavan",
            ["tavanların"] = "tavan",
            ["asma tavan"] = "tavan",
            ["boru"] = "boru",
            ["boruların"] = "boru",
            ["sistem"] = "sistem",
            ["sisteminin"] = "sistem",
            ["drenaj"] = "drenaj",
            ["havalandırma"] = "havalandırma",
            ["metal"] = "metal",
            ["dəmir"] = "metal",
            ["çən"] = "çən",
            ["çəni"] = "çən",
            ["pəncərə"] = "pəncərə",
            ["pəncərələrin"] = "pəncərə",
            ["plastik"] = "pəncərə",
            ["qapıların"] = "qapı",
            ["taxta"] = "taxta",
            ["plitə"] = "plitə",
            ["şüşə"] = "şüşə",
            ["şifer"] = "şifer",
            ["polikarbonat"] = "polikarbonat",
            ["elektrik"] = "elektrik",
            ["santexnik"] = "santexnik",
            ["santexnika"] = "santexnik",
            ["aboy"] = "aboy",
            ["divar kağızı"] = "aboy",
            ["divar kagizi"] = "aboy",
            ["paduqa"] = "paduqa",
            ["işləri"] = "",
            ["işi"] = "",
            ["qiyməti"] = "",
            ["neçəyədir"] = "",
            ["axtarıram"] = "",
            ["lazımdır"] = "",
            ["olunması"] = "",
            ["otağın"] = "otaq",
            ["evin"] = "ev",
            ["mənzildə"] = "ev",
            ["hamam"] = "hamam",
            ["mətbəx"] = "mətbəx",
            ["isti pol"] = "isti pol",
            ["ustasi"] = "usta",
            ["profilsiz"] = "profilsiz",
            ["şitinin"] = "şit",
            ["Trapın"] = "Trap"
        };


        private static readonly string[] GenericRaw =
        {
            "quraşdırma", "qurulma", "vurulma", "verilməsi", "çəkilmə",
    "sökülmə", "montaj", "demontaj", "daşınma", "mm",
    "quraşdırıl", "diametri", "mm-ə", "quraşdır"
        };
        private static readonly string[] CriticalRaw =
        {
             "kombi", "təmir", "bərpa", "remont", "quraşdırma",
    "montaj", "demontaj", "söküntü", "izolyasiya", "boyanma",
    "rənglənmə", "vurulma", "çəkilmə", "tamet", "profilsiz",
    "izolyasiyalı", "şit","qapı"
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
