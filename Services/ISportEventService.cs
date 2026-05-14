using SportsAIPredictionAPI.DTOs;

namespace SportsAIPredictionAPI.Services;

public interface ISportEventService
{
    Task<IEnumerable<SportEventDto>> GetAllAsync();
    Task<SportEventDto?> GetByIdAsync(int id);
    Task<SportEventDto> CreateAsync(CreateSportEventRequest request);
    Task<bool> DeleteAsync(int id);
}
