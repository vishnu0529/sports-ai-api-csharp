using System.ComponentModel.DataAnnotations;

namespace SportsAIPredictionAPI.DTOs;

public class PredictionDto
{
    public int Id { get; set; }
    public int SportEventId { get; set; }
    public string Sport { get; set; } = string.Empty;
    public string HomeTeam { get; set; } = string.Empty;
    public string AwayTeam { get; set; } = string.Empty;
    public string PredictedOutcome { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreatePredictionRequest
{
    [Required]
    public int SportEventId { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string HomeTeam { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string AwayTeam { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Sport { get; set; } = string.Empty;
}
