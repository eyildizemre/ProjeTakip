using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProjeTakip.Models.ViewModels
{
    public class ProjectVM
    {
        public int ProjectId { get; set; }

        [Required]
        [MaxLength(200)]
        [Display(Name = "Proje İsmi")]
        public string ProjectName { get; set; }

        [Required]
        [Display(Name = "Proje Açıklaması")]
        public string ProjectDescription { get; set; }

        [Display(Name = "Ekip")]
        public int? TeamId { get; set; }

        [ValidateNever]
        public string TeamName { get; set; } // TeamName eklendi

        [Display(Name = "Ekip Lideri")]
        public int? TeamLeadId { get; set; }

        [ValidateNever]
        public string TeamLeadName { get; set; } // TeamLeadName eklendi

        [Required]
        [Display(Name = "Başlangıç Tarihi")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Bitiş Tarihi")]
        public DateTime EndDate { get; set; }

        public int? ProjectStatusId { get; set; }

        [ValidateNever]
        public string ProjectStatusName { get; set; } // Proje durumunun ismi

        [ValidateNever]
        [Display(Name = "Onay Durumu")]
        public int OnayDurumuId { get; set; }  // Onay Durumu ID

        [ValidateNever]
        [Display(Name = "Onay Durumu")]
        public string OnayDurumuAdi { get; set; }  // Onay Durumu ismi

        public bool Enabled { get; set; }

        [ValidateNever]
        public string Comment { get; set; } // Admin'in gireceği yorum

        [ValidateNever]
        public bool IsApproved { get; set; } // Proje onay durumu

        // Yeni eklenen özellikler
        [ValidateNever]
        public int TeamCapacity { get; set; } // Ekip kapasitesi

        [ValidateNever]
        public int TotalTasks { get; set; } // Toplam görev sayısı

        [ValidateNever]
        public int SuccessfulTasks { get; set; } // Başarılı görev sayısı

        [ValidateNever]
        public int FailedTasks { get; set; } // Başarısız görev sayısı

        // Dropdowns için gerekli listeler
        [ValidateNever]
        public IEnumerable<SelectListItem> Teams { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> TeamLeads { get; set; }
    }
}