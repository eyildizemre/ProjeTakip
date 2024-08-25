using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeTakip.Models.ViewModels
{
    public class ProfileVM
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Adınızı girmelisiniz.")]
        [Display(Name = "Ad")]
        public string UserFName { get; set; }

        [Required(ErrorMessage = "Soyadınızı girmelisiniz.")]
        [Display(Name = "Soyad")]
        public string UserLName { get; set; }

        [Required(ErrorMessage = "E-posta adresinizi girmelisiniz.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-posta")]
        public string Email { get; set; }

        [Display(Name = "GitHub Profili")]
        public string GitHubProfile { get; set; }
    }
}
