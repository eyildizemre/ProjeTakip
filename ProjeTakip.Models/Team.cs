using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeTakip.Models
{
    public class Team
    {
        [Key]
        public int TeamId { get; set; }

        [MaxLength(100)]
        [Display(Name = "Ekip Adı")]
        public string TeamName { get; set; }

        [ForeignKey("UserId")]
        [ValidateNever]  // Validation sırasında göz ardı edilir
        public int TeamLeadId { get; set; }

        [ValidateNever]  // Validation sırasında göz ardı edilir
        public User TeamLead { get; set; } // TeamLead ile ilişkiyi tanımlayan navigasyon property

        [ForeignKey("ProjectId")]
        [ValidateNever]  // Validation sırasında göz ardı edilir
        public int? ProjectId { get; set; }

        [ValidateNever]  // Validation sırasında göz ardı edilir
        public Project Project { get; set; } // Project ile ilişkiyi tanımlayan navigasyon property

        public int? Capacity { get; set; } // Ekip kapasitesi (TeamLead dâhil)

        public bool Enabled { get; set; }

        // Navigasyon özellikleri
        [ValidateNever]  // Validation sırasında göz ardı edilir
        public ICollection<Project> Projects { get; set; }

        [ValidateNever]  // Validation sırasında göz ardı edilir
        public ICollection<UserTeam> UserTeams { get; set; } // Ekip ve kullanıcılar arasındaki ilişki
    }
}
