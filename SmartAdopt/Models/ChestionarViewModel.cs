using System.ComponentModel.DataAnnotations;

public class ChestionarViewModel
{
    [Required]
    public string Locuinta { get; set; } 
    [Required]
    public bool GradinaBool { get; set; }
    [Required]
    public string TimpMiscare { get; set; } 
    [Required]
    public bool AnimaleBool { get; set; }
    [Required]
    public bool CopiiBool { get; set; }
    [Required]
    [Range(1, 5)]
    public int Marime { get; set; }
    [Required]
    [Range(1, 5)]
    public int NivelAtentie { get; set; }
    [Required]
    [Range(1, 5)]
    public int GrupVarsta { get; set; }
    [Required]
    [Range(1, 5)]
    public int Adaptabilitate { get; set; }
}