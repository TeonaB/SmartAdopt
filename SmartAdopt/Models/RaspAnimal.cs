using System.ComponentModel.DataAnnotations;

namespace SmartAdopt.Models
{
    public class RaspAnimal
    {
        [Key]
        public int idRaspAnimal { get; set; }
        public int idRasp { get; set; }
        public virtual RaspChestionar? RaspChestionar { get; set; }
        public int idAnimal { get; set; }
        public virtual Animal? Animal { get; set; }
    }
}
