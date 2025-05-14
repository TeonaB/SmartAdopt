using System.ComponentModel.DataAnnotations;

namespace SmartAdopt.Models
{
    public class Postare
    {
        [Key]
        public int idPostare { get; set; }
        public string ApplicationUserId { get; set; }
        [Required(ErrorMessage = "Titlul este obligatoriu!")]
        public string titlu { get; set; }
        [Required(ErrorMessage = "Descrierea este obligatorie!")]
        public string descriere { get; set; }
        public DateTime data_postarii { get; set; } 

        // Navigation properties
        public virtual ApplicationUser? ApplicationUser { get; set; }
        public virtual ICollection<Comentariu>? Comentarius { get; set; }
    }
}
