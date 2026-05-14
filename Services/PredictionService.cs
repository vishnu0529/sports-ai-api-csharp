using Microsoft.EntityFrameworkCore;
using SportsAIPredictionAPI.Data;
using SportsAIPredictionAPI.DTOs;
using SportsAIPredictionAPI.Models;

namespace SportsAIPredictionAPI.Services;

public class PredictionService : IPredictionService
{
    private readonly AppDbContext _context;
    private readonly IAIService _aiService;
    private readonly ILogger<PredictionService> _logger;

    public PredictionService(AppDbContext context, IAIService aiService, ILogger<PredictionService> logger)
    {
        _context = context;
        _aiService = aiService;
        _logger = logger;
    }

    public async Task<IEnumerable<PredictionDto>> GetUserPredictionsAsync(int userId)
    {
        _logger.LogInformation("Fetching predictions for user {UserId}", userId);
        return await _context.Predictions
            .Include(p => p.SportEvent)
            .Where(p => p.UserId == userId)
            .Select(p => new PredictionDto
            {
                Id = p.Id,
                SportEventId = p.SportEventId,
                Sport = p.SportEvent!.Sport,
                HomeTeam = p.SportEvent.HomeTeam,
                AwayTeam = p.SportEvent.AwayTeam,
                PredictedOutcome = p.PredictedOutcome,
                Confidence = p.Confidence,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<PredictionDto?> GetByIdAsync(int id, int userId)
    {
        var p = await _context.Predictions
            .Include(p => p.SportEvent)
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

        if (p == null) return null;

        return new PredictionDto
        {
            Id = p.Id,
            SportEventId = p.SportEventId,
            Sport = p.SportEvent!.Sport,
            HomeTeam = p.SportEvent.HomeTeam,
            AwayTeam = p.SportEvent.AwayTeam,
            PredictedOutcome = p.PredictedOutcome,
            Confidence = p.Confidence,
            CreatedAt = p.CreatedAt
        };
    }

    public async Task<PredictionDto> CreateAsync(CreatePredictionRequest request, int userId)
    {
        var (outcome, confidence) = await _aiService.PredictAsync(
            request.HomeTeam, request.AwayTeam, request.Sport);

        var prediction = new Prediction
        {
            UserId = userId,
            SportEventId = request.SportEventId,
            PredictedOutcome = outcome,
            Confidence = confidence,
            CreatedAt = DateTime.UtcNow
        };

        _context.Predictions.Add(prediction);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created prediction {Id} for user {UserId}", prediction.Id, userId);

        return new PredictionDto
        {
            Id = prediction.Id,
            SportEventId = prediction.SportEventId,
            Sport = request.Sport,
            HomeTeam = request.HomeTeam,
            AwayTeam = request.AwayTeam,
            PredictedOutcome = prediction.PredictedOutcome,
            Confidence = prediction.Confidence,
            CreatedAt = prediction.CreatedAt
        };
    }
}
