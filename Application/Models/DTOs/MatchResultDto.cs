namespace Application.Models.DTOs
{
    public record MatchResultDto(string MasterText,
                             double Price,
                             string Unit,
                             int Score);
}
