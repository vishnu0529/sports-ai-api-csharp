namespace SportsAIPredictionAPI.Services;

public interface IAIService
{
    Task<(string outcome, double confidence)> PredictAsync(string homeTeam, string awayTeam, string sport);
}
