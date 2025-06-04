using System.ComponentModel.DataAnnotations;

namespace SmartAdopt.Models
{
    public class RaspChestionar
    {
        [Key]
        public int idRasp { get; set; }
        public int idClient { get; set; }
        public virtual Client? Client { get; set; }


        public virtual ICollection<RaspAnimal>? RaspAnimals { get; set; }
    }
}
