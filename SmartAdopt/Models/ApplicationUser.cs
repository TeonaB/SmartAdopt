using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAdopt.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string nume { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string prenume { get; set; }

        public virtual ICollection<Postare>? Postares { get; set; }

    }
}
