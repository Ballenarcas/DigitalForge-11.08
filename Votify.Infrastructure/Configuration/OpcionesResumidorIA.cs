namespace Votify.Infrastructure.Configuration
{
    public class OpcionesResumidorIA
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string Model { get; set; } = "gemini-3-flash";
        public int MaxTokens { get; set; } = 500;
        public int TimeoutSeconds { get; set; } = 30;
        public int MaxRetryAttempts { get; set; } = 3;
        public int RetryDelaySeconds { get; set; } = 2;
        public bool Enabled { get; set; } = false;
    }
}