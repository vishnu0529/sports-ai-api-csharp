using System.ComponentModel.DataAnnotations;

namespace SportsAIPredictionAPI.DTOs;

public class SportEventDto
{
    public int Id { get; set; }
    public string Sport { get; set; } = string.Empty;
    public string HomeTeam { get; set; } = string.Empty;
    public string AwayTeam { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class CreateSportEventRequest
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Sport { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string HomeTeam { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string AwayTeam { get; set; } = string.Empty;

    [Required]
    [FutureDate]
    public DateTime EventDate { get; set; }
}

public class FutureDateAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        if (value is DateTime date && date <= DateTime.UtcNow)
            return new ValidationResult("Event date must be in the future.");
        return ValidationResult.Success;
    }
}
