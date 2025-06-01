using System.ComponentModel.DataAnnotations;

public class ChestionarViewModel
{
    [Required]
    public string LivingSituation { get; set; } 
    [Required]
    public bool HasYard { get; set; }
    [Required]
    public string ExerciseTime { get; set; } 
    [Required]
    public bool HasOtherPets { get; set; }
    [Required]
    public bool HasChildren { get; set; }
    [Required]
    [Range(1, 5)]
    public int PreferredSize { get; set; }
    [Required]
    [Range(1, 5)]
    public int AttentionLevel { get; set; }
    [Required]
    [Range(1, 5)]
    public int ExperienceLevel { get; set; }
    [Required]
    [Range(1, 5)]
    public int PreferredAgeGroup { get; set; }
    [Required]
    [Range(1, 5)]
    public int AdaptabilityImportance { get; set; }
}