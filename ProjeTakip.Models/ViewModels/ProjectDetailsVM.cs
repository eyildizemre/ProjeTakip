using System;
using System.Collections.Generic;

namespace ProjeTakip.Models.ViewModels
{
    public class ProjectDetailsVM
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public string TeamName { get; set; }
        public string TeamLeadName { get; set; }
        public int TeamCapacity { get; set; }
        public int TotalTasks { get; set; }  // Mevcut Task sayısı
        public int SuccessfulTasks { get; set; }  // Başarılı Task sayısı
        public int FailedTasks { get; set; }  // Başarısız Task sayısı
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ProjectStatusName { get; set; }
    }
}