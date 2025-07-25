namespace Application.Models.DTOs
{
    public class MasterRow(string originalText, string canonText, string flag, string unit, double price,
                 HashSet<string> tokenSet, string? material, double priceMedian)
    {
        public string OriginalText { get; } = originalText;
        public string CanonText { get; } = canonText;
        public string Flag { get; } = flag;
        public string Unit { get; } = unit;
        public double Price { get; } = price;
        public HashSet<string> TokenSet { get; } = tokenSet;
        public string? Material { get; } = material;
        public double PriceMedian { get; set; } = priceMedian;
    }
}
