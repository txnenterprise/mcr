namespace MCR.API.Services
{
    public class JwtSettings
    {
        public string Secret { get; set; }
        public int ExpirationInHours { get; set; } = 8;
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
