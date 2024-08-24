using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ProjeTakip.Models.ViewModels
{
    public class TaskVM
    {
        public int TaskId { get; set; }

        [Required(ErrorMessage = "Görev ismi gereklidir.")]
        [MaxLength(200)]
        [Display(Name = "Görev İsmi")]
        public string TaskName { get; set; }

        [Display(Name = "Görev Açıklaması")]
        public string? TaskDescription { get; set; }

        [Required(ErrorMessage = "Bir kullanıcı seçilmelidir.")]
        [Display(Name = "Atanan Kullanıcı")]
        public int? UserId { get; set; }

        public IEnumerable<SelectListItem> Users { get; set; } = new List<SelectListItem>();

        [ValidateNever]
        [Display(Name = "Atanan Kullanıcı")]
        public string UserFullName { get; set; } // Yeni özellik

        [Required(ErrorMessage = "Bir proje seçilmelidir.")]
        [Display(Name = "Proje")]
        public int ProjectId { get; set; }

        public IEnumerable<SelectListItem> Projects { get; set; } = new List<SelectListItem>();

        [ValidateNever]
        [Display(Name = "Proje")]
        public string ProjectName { get; set; } // Yeni özellik

        [Required(ErrorMessage = "Bir durum seçilmelidir.")]
        [Display(Name = "Görev Durumu")]
        public int TaskStatusId { get; set; } = 2;

        public IEnumerable<SelectListItem> Statuses { get; set; } = new List<SelectListItem>();

        [ValidateNever]
        [Display(Name = "Görev Durumu")]
        public string TaskStatusName { get; set; } // Yeni özellik

        [Display(Name = "Başlangıç Tarihi")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "Bitiş Tarihi")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [MaxLength(255)]
        [Display(Name = "GitHub Push URL")]
        public string? GitHubPush { get; set; }
    }
}
