using System.ComponentModel.DataAnnotations;

namespace SmartAdopt.Models
{
    public class AnimalAdoptat
    {
        [Key]
        public int idAnimalAdoptat { get; set; }

        public int counter { get; set; }
    }
}
