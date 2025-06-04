using System.ComponentModel.DataAnnotations;

namespace SmartAdopt.Models
{
    public class Comanda
    {
        [Key]
        public int idComanda { get; set; }
        public int idClient { get; set; }
        public virtual Client? Client { get; set; }
        public int idAnimal { get; set; }
        public virtual Animal? Animal { get; set; }
        public string stare { get; set; }
        public DateTime data_comenzii { get; set; } 
        public decimal total_plata { get; set; } 
        public string metoda_platii { get; set; }
        [Required(ErrorMessage = "Motivatia este obligatorie!")]
        public string motivatie { get; set; }
    }
}
