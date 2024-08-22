using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [Display(Name = "Ekip Lideri")]
        public int? TeamLeadId { get; set; }

        [Required]
        [Display(Name = "Başlangıç Tarihi")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Bitiş Tarihi")]
        public DateTime EndDate { get; set; }

        public int? ProjectStatusId { get; set; }

        public bool Enabled { get; set; }

        // Durumun ismi, UI'da göstermek için kullanılabilir
        public string ProjectStatusName { get; set; }

        // Dropdowns için gerekli listeler
        public IEnumerable<SelectListItem> Teams { get; set; }
        public IEnumerable<SelectListItem> TeamLeads { get; set; }
    }

}