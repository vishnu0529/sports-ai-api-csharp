using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsAIPredictionAPI.DTOs;
using SportsAIPredictionAPI.Services;

namespace SportsAIPredictionAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SportEventController : ControllerBase
{
    private readonly ISportEventService _service;
    private readonly ILogger<SportEventController> _logger;

    public SportEventController(ISportEventService service, ILogger<SportEventController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>Get all sport events</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var events = await _service.GetAllAsync();
        return Ok(events);
    }

    /// <summary>Get a sport event by ID</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var sportEvent = await _service.GetByIdAsync(id);
        if (sportEvent == null)
            return NotFound(new { message = $"Sport event {id} not found." });
        return Ok(sportEvent);
    }

    /// <summary>Create a new sport event</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSportEventRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Delete a sport event</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { message = $"Sport event {id} not found." });
        return NoContent();
    }
}
