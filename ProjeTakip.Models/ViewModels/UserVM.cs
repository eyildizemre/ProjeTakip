using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeTakip.Models.ViewModels
{
    public class UserVM
    {
        [Required]
        [Display(Name = "Kullanıcı Adı")]
        public string UserFName { get; set; }

        [Required]
        [Display(Name = "Kullanıcı Soyadı")]
        public string UserLName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "E-posta")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        // Rolleri seçmek için bir liste
        public IEnumerable<SelectListItem> Roles { get; set; }
    }
}
