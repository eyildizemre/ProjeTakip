using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjeTakip.Models.ViewModels
{
    public class UserVM
    {
        public int UserId { get; set; }  // Update işlemi için gerekli

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

        // Admin tarafından kullanıcı oluşturuluyorken şifre otomatik olarak oluşturulacağı için artık ihtiyaç yok.
        //[Required]
        //[DataType(DataType.Password)]
        //[Display(Name = "Şifre")]
        //public string Password { get; set; }  // Update işleminde gerekli olmayabilir
        //[ValidateNever]

        public string? GitHubProfile { get; set; }

        [Required]
        public int RoleId { get; set; }

        public bool Enabled { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> Roles { get; set; }
    }
}