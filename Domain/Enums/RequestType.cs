namespace Domain.Enums
{
    public enum RequestType
    {
        PlanInqury = 1,
        Support = 2,
        Registration = 3,
        ComplaintAndSuggestion = 4
    }

    public static class RequestTypeExtensions
    {
        public static RequestType Parse(string value)
        {
            if (Enum.TryParse<RequestType>(value, ignoreCase: true, out var result))
                return result;

            var accepted = string.Join(", ", Enum.GetNames(typeof(RequestType)));
            throw new ArgumentException($"Invalid request type '{value}'. Accepted values are: {accepted}.");
        }
    }
}