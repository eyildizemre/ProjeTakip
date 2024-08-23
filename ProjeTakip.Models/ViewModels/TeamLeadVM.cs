using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeTakip.Models.ViewModels
{
    public class TeamLeadVM
    {
        public string TeamLeadName { get; set; } // Giriş yapan TeamLead'in adı
        public List<CalendarEvent> CalendarEvents { get; set; } // Takvimde gösterilecek olaylar
        public IEnumerable<SelectListItem> ProjectList { get; set; } // Projeler için dropdown
        public IEnumerable<SelectListItem> TaskList { get; set; } // Görevler için dropdown
        public List<TeamMemberTaskVM> PendingTasks { get; set; } // Onay bekleyen görevler
        public TeamLeadVM()
        {
            CalendarEvents = new List<CalendarEvent>();
            PendingTasks = new List<TeamMemberTaskVM>();
        }
    }

    public class TeamMemberTaskVM
    {
        public int TaskId { get; set; }

        [Display(Name = "Görev Adı")]
        public string TaskName { get; set; }

        [Display(Name = "Açıklama")]
        public string TaskDescription { get; set; }

        [Display(Name = "Atanan Üye")]
        public string AssignedMemberName { get; set; }

        [Display(Name = "Durum")]
        public string TaskStatusName { get; set; }

        [Display(Name = "Tamamlanma Tarihi")]
        public DateTime? CompletionDate { get; set; }
    }
}
