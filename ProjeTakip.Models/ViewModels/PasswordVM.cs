using System.ComponentModel.DataAnnotations;

namespace ProjeTakip.Models.ViewModels
{
    public class PasswordVM
    {
        [Required(ErrorMessage = "Mevcut şifrenizi girmelisiniz.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mevcut Şifre")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Yeni şifrenizi girmelisiniz.")]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre")]
        [MinLength(8, ErrorMessage = "Şifreniz en az 8 karakter olmalıdır.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Yeni şifrenizi onaylamalısınız.")]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifreyi Onayla")]
        [Compare("NewPassword", ErrorMessage = "Yeni şifreleriniz eşleşmiyor.")]
        public string ConfirmPassword { get; set; }
    }
}