using System.ComponentModel.DataAnnotations;

namespace SmartAdopt.Models
{
    public class Comentariu
    {
        [Key]
        public int idComentariu { get; set; }
        public int idClient { get; set; }
        public virtual Client? Client { get; set; }
        public int idPostare { get; set; }
        public virtual Postare? Postare { get; set; }
        [Required(ErrorMessage = "Descrierea este obligatorie!")]
        public string descriere { get; set; }   
    }
}
