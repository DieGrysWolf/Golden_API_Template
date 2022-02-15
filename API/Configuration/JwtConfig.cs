namespace API.Configuration
{
    public class JwtConfig
    {
        public string? Secret { get; set; }
        public TimeSpan ExpiryTimeFrame { get; set; } = TimeSpan.FromMinutes(5);
    }
}
