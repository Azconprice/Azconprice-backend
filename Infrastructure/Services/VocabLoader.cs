using System.Text.Json;


namespace Infrastructure.Services
{
    public sealed class Vocab
    {
        public Dictionary<string, string> Synonyms { get; init; } = new();
        public HashSet<string> Generic { get; init; } = new();
        public HashSet<string> Critical { get; init; } = new();
    }


    public static class VocabLoader
    {
        public static Vocab Load(string path)
        {
            return JsonSerializer.Deserialize<Vocab>(File.ReadAllText(path))
                   ?? throw new InvalidOperationException("vocab.json unreadable");
        }
    }
}
