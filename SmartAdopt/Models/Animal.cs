using System.ComponentModel.DataAnnotations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SmartAdopt.Models
{
    public class Animal
    {
        [Key]
        public int idAnimal { get; set; }

        [Required(ErrorMessage = "Numele este obligatoriu!")]
        public string nume { get; set; }
        [Required(ErrorMessage = "Specia este obligatorie!")]
        public string specie { get; set; }
        [Required(ErrorMessage = "Rasa este obligatorie!")]
        public string rasa { get; set; }
        [Required(ErrorMessage = "Descrierea este obligatorie!")]
        public string descriere { get; set; }
        [Required(ErrorMessage = "Varsta este obligatorie!")]
        [Range(0, 20, ErrorMessage = "Introdu o valoarea reala")]
        public int varsta { get; set; }
        [Required(ErrorMessage = "Camp obligatoriu!")]
        [Range(0, 5, ErrorMessage = "Introdu o valoare intre 0 si 5")]
        public int nivel_energie { get; set; }
        [Required(ErrorMessage = "Camp obligatoriu!")]
        [Range(0, 5, ErrorMessage = "Introdu o valoare intre 0 si 5")]
        public int marime { get; set; }
        [Required(ErrorMessage = "Camp obligatoriu!")]
        [Range(0, 5, ErrorMessage = "Introdu o valoare intre 0 si 5")]
        public int nivel_adaptabilitate { get; set; }
        [Required(ErrorMessage = "Camp obligatoriu!")]
        [Range(0, 5, ErrorMessage = "Introdu o valoare intre 0 si 5")]
        public int grupa_varsta { get; set; }
        [Required(ErrorMessage = "Camp obligatoriu!")]
        [Range(0, 5, ErrorMessage = "Introdu o valoare intre 0 si 5")]
        public int nivel_atentie_necesara { get; set; }
        public string vaccinuri { get; set; } 
        public string tip_alimentatie { get; set; } 
        public string starea_sanatatii { get; set; }
        public DateTime ultima_verificare_vet { get; set; }
        [Required(ErrorMessage = "Camp obligatoriu!")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Introdu o valoare > 0!")]
        public decimal pret { get; set; } 

        // Navigation properties
        public virtual ICollection<RaspAnimal>? RaspAnimals { get; set; }
        public virtual ICollection<Comanda>? Comandas { get; set; }
    }
}
