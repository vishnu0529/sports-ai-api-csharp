using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsAIPredictionAPI.DTOs;
using SportsAIPredictionAPI.Services;
using System.Security.Claims;

namespace SportsAIPredictionAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PredictionController : ControllerBase
{
    private readonly IPredictionService _service;
    private readonly ILogger<PredictionController> _logger;

    public PredictionController(IPredictionService service, ILogger<PredictionController> logger)
    {
        _service = service;
        _logger = logger;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Get all predictions for the logged in user</summary>
    [HttpGet]
    public async Task<IActionResult> GetMyPredictions()
    {
        var predictions = await _service.GetUserPredictionsAsync(GetUserId());
        return Ok(predictions);
    }

    /// <summary>Get a single prediction by ID</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var prediction = await _service.GetByIdAsync(id, GetUserId());
        if (prediction == null)
            return NotFound(new { message = $"Prediction {id} not found." });
        return Ok(prediction);
    }

    /// <summary>Create a new AI prediction for a sport event</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePredictionRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var prediction = await _service.CreateAsync(request, GetUserId());
        return CreatedAtAction(nameof(GetById), new { id = prediction.Id }, prediction);
    }
}
