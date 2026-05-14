using Microsoft.EntityFrameworkCore;
using SportsAIPredictionAPI.Data;
using SportsAIPredictionAPI.DTOs;
using SportsAIPredictionAPI.Models;

namespace SportsAIPredictionAPI.Services;

public class SportEventService : ISportEventService
{
    private readonly AppDbContext _context;
    private readonly ILogger<SportEventService> _logger;

    public SportEventService(AppDbContext context, ILogger<SportEventService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<SportEventDto>> GetAllAsync()
    {
        _logger.LogInformation("Fetching all sport events");
        return await _context.SportEvents
            .Select(e => new SportEventDto
            {
                Id = e.Id,
                Sport = e.Sport,
                HomeTeam = e.HomeTeam,
                AwayTeam = e.AwayTeam,
                EventDate = e.EventDate,
                Status = e.Status
            })
            .ToListAsync();
    }

    public async Task<SportEventDto?> GetByIdAsync(int id)
    {
        var e = await _context.SportEvents.FindAsync(id);
        if (e == null) return null;
        return new SportEventDto
        {
            Id = e.Id,
            Sport = e.Sport,
            HomeTeam = e.HomeTeam,
            AwayTeam = e.AwayTeam,
            EventDate = e.EventDate,
            Status = e.Status
        };
    }

    public async Task<SportEventDto> CreateAsync(CreateSportEventRequest request)
    {
        var sportEvent = new SportEvent
        {
            Sport = request.Sport,
            HomeTeam = request.HomeTeam,
            AwayTeam = request.AwayTeam,
            EventDate = request.EventDate,
            Status = "Upcoming"
        };

        _context.SportEvents.Add(sportEvent);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created sport event {Id}", sportEvent.Id);

        return new SportEventDto
        {
            Id = sportEvent.Id,
            Sport = sportEvent.Sport,
            HomeTeam = sportEvent.HomeTeam,
            AwayTeam = sportEvent.AwayTeam,
            EventDate = sportEvent.EventDate,
            Status = sportEvent.Status
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var sportEvent = await _context.SportEvents.FindAsync(id);
        if (sportEvent == null) return false;
        _context.SportEvents.Remove(sportEvent);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Deleted sport event {Id}", id);
        return true;
    }
}
