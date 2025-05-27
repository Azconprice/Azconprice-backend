namespace Application.Models
{
    public class SupabaseSettings
    {
        public string Url { get; set; } = default!;  // https://xxxx.supabase.co
        public string ApiKey { get; set; } = default!;  // SERVICE-ROLE key recommended for server
        public string BucketName { get; set; } = "profile-pics";
    }
}
