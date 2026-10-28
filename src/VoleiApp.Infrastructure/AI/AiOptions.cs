namespace VoleiApp.Infrastructure.AI
{
    public class AiOptions
    {
        public const string SectionName = "Ai";

        public string BaseUrl { get; set; } = "http://localhost:11434";
        public string Model { get; set; } = "llama3.1:latest";
        public string ApiKey { get; set; } = string.Empty;
        public int TimeoutSeconds { get; set; } = 20;
        public int MaxInputChars { get; set; } = 1200;
        public int MaxOutputTokens { get; set; } = 220;
    }
}
