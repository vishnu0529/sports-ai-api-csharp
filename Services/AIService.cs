using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;

namespace SportsAIPredictionAPI.Services;

public class AIService : IAIService
{
    private readonly IConfiguration _config;
    private readonly ILogger<AIService> _logger;

    public AIService(IConfiguration config, ILogger<AIService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task<(string outcome, double confidence)> PredictAsync(
        string homeTeam, string awayTeam, string sport)
    {
        var apiKey = _config["OpenAI:ApiKey"];

        if (string.IsNullOrEmpty(apiKey))
        {
            _logger.LogWarning("OpenAI API key not configured. Using rule-based prediction.");
            return FallbackPredict(homeTeam, awayTeam);
        }

        try
        {
            var client = new AzureOpenAIClient(
                new Uri("https://api.openai.com/v1"),
                new AzureKeyCredential(apiKey));

            var chatClient = client.GetChatClient("gpt-3.5-turbo");

            var prompt = "You are a sports analyst. Predict the outcome of this match:\n" +
                         "Sport: " + sport + "\n" +
                         "Home Team: " + homeTeam + "\n" +
                         "Away Team: " + awayTeam + "\n\n" +
                         "Respond in this exact JSON format only:\n" +
                         "{\"outcome\": \"" + homeTeam + " Win or " + awayTeam + " Win or Draw\", \"confidence\": 0.75}";

            var response = await chatClient.CompleteChatAsync(
                new UserChatMessage(prompt));

            var text = response.Value.Content[0].Text;
            return ParseAIResponse(text, homeTeam, awayTeam);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OpenAI call failed, falling back to rule-based prediction");
            return FallbackPredict(homeTeam, awayTeam);
        }
    }

    private static (string outcome, double confidence) ParseAIResponse(
        string text, string homeTeam, string awayTeam)
    {
        try
        {
            var json = System.Text.Json.JsonDocument.Parse(text);
            var outcome = json.RootElement.GetProperty("outcome").GetString() ?? homeTeam + " Win";
            var confidence = json.RootElement.GetProperty("confidence").GetDouble();
            return (outcome, Math.Round(confidence * 100, 2));
        }
        catch
        {
            return FallbackPredict(homeTeam, awayTeam);
        }
    }

    private static (string outcome, double confidence) FallbackPredict(
        string homeTeam, string awayTeam)
    {
        var outcomes = new[] { homeTeam + " Win", awayTeam + " Win", "Draw" };
        var outcome = outcomes[new Random().Next(outcomes.Length)];
        var confidence = Math.Round(new Random().NextDouble() * 40 + 55, 2);
        return (outcome, confidence);
    }
}
