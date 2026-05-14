using SportsAIPredictionAPI.DTOs;

namespace SportsAIPredictionAPI.Services;

public interface IPredictionService
{
    Task<IEnumerable<PredictionDto>> GetUserPredictionsAsync(int userId);
    Task<PredictionDto?> GetByIdAsync(int id, int userId);
    Task<PredictionDto> CreateAsync(CreatePredictionRequest request, int userId);
}
