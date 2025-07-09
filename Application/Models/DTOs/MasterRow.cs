namespace Application.Models.DTOs
{
    public record MasterRow(string OriginalText,
                          string CanonText,
                          string Flag,
                          string Unit,
                          double Price,
                          HashSet<string> TokenSet);
}
