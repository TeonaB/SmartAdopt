using System.ComponentModel.DataAnnotations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SmartAdopt.Models
{
    public class Client
    {
        [Key]
        public int idClient { get; set; }
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public int idRaspChestionar { get; set; }
        public virtual RaspChestionar? RaspChestionar { get; set; }
        public bool CompletedProfile { get; set; }
        public string nr_telefon { get; set; }
        public string adresa { get; set; }

        // Navigation properties
        public virtual ICollection<Comanda>? Comandas { get; set; }
        public virtual ICollection<Comentariu>? Comentarius { get; set; }
    }
}
