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
        [ValidateNever]
        public int? UserId { get; set; }

        public IEnumerable<SelectListItem> Users { get; set; } = new List<SelectListItem>();

        [ValidateNever]
        [Display(Name = "Atanan Kullanıcı")]
        public string AssignedUserName { get; set; } // Atanan Kullanıcının adı ve soyadı

        [Required(ErrorMessage = "Bir proje seçilmelidir.")]
        [Display(Name = "Proje")]
        public int ProjectId { get; set; }

        public IEnumerable<SelectListItem> Projects { get; set; } = new List<SelectListItem>();

        [ValidateNever]
        [Display(Name = "Proje")]
        public string ProjectName { get; set; } // Proje ismi

        [Required(ErrorMessage = "Bir durum seçilmelidir.")]
        [Display(Name = "Görev Durumu")]
        public int TaskStatusId { get; set; } = 2;

        public IEnumerable<SelectListItem> Statuses { get; set; } = new List<SelectListItem>();

        [Display(Name = "Yorum")]
        public string? CommentText { get; set; } // Yorum için TextArea alanı

        [ValidateNever]
        [Display(Name = "Görev Durumu")]
        public string TaskStatusName { get; set; } // Görev Durumu ismi

        [ValidateNever]
        [Display(Name = "Onay Durumu")]
        public int OnayDurumuId { get; set; }  // Onay Durumu ID

        [ValidateNever]
        [Display(Name = "Onay Durumu")]
        public string OnayDurumuAdi { get; set; }  // Onay Durumu ismi

        [ValidateNever]
        [Display(Name = "Görev Veren")]
        public string TeamLeadName { get; set; } // Görevi veren TeamLead'in adı ve soyadı

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
