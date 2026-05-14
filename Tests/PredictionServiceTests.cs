using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SportsAIPredictionAPI.Data;
using SportsAIPredictionAPI.DTOs;
using SportsAIPredictionAPI.Models;
using SportsAIPredictionAPI.Services;
using Xunit;

namespace SportsAIPredictionAPI.Tests;

public class PredictionServiceTests
{
    private AppDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnPrediction_WhenValidRequest()
    {
        var context = GetInMemoryContext();
        var mockAI = new Mock<IAIService>();
        var mockLogger = new Mock<ILogger<PredictionService>>();

        mockAI.Setup(x => x.PredictAsync("Arsenal", "Chelsea", "Football"))
              .ReturnsAsync(("Arsenal Win", 75.0));

        context.SportEvents.Add(new SportEvent
        {
            Id = 1,
            Sport = "Football",
            HomeTeam = "Arsenal",
            AwayTeam = "Chelsea",
            EventDate = DateTime.UtcNow.AddDays(7),
            Status = "Upcoming"
        });
        await context.SaveChangesAsync();

        var service = new PredictionService(context, mockAI.Object, mockLogger.Object);

        var request = new CreatePredictionRequest
        {
            SportEventId = 1,
            HomeTeam = "Arsenal",
            AwayTeam = "Chelsea",
            Sport = "Football"
        };

        var result = await service.CreateAsync(request, userId: 1);

        Assert.NotNull(result);
        Assert.Equal("Arsenal Win", result.PredictedOutcome);
        Assert.Equal(75.0, result.Confidence);
    }

    [Fact]
    public async Task GetUserPredictionsAsync_ShouldReturnOnlyUserPredictions()
    {
        var context = GetInMemoryContext();
        var mockAI = new Mock<IAIService>();
        var mockLogger = new Mock<ILogger<PredictionService>>();

        context.SportEvents.Add(new SportEvent
        {
            Id = 1,
            Sport = "Football",
            HomeTeam = "Arsenal",
            AwayTeam = "Chelsea",
            EventDate = DateTime.UtcNow.AddDays(7),
            Status = "Upcoming"
        });

        context.Predictions.AddRange(
            new Prediction { UserId = 1, SportEventId = 1, PredictedOutcome = "Arsenal Win", Confidence = 70.0, CreatedAt = DateTime.UtcNow },
            new Prediction { UserId = 2, SportEventId = 1, PredictedOutcome = "Chelsea Win", Confidence = 65.0, CreatedAt = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();

        var service = new PredictionService(context, mockAI.Object, mockLogger.Object);

        var results = await service.GetUserPredictionsAsync(userId: 1);

        Assert.Single(results);
        Assert.Equal("Arsenal Win", results.First().PredictedOutcome);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenPredictionNotFound()
    {
        var context = GetInMemoryContext();
        var mockAI = new Mock<IAIService>();
        var mockLogger = new Mock<ILogger<PredictionService>>();

        var service = new PredictionService(context, mockAI.Object, mockLogger.Object);

        var result = await service.GetByIdAsync(id: 999, userId: 1);

        Assert.Null(result);
    }
}
