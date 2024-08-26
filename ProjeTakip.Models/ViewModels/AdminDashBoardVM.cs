using ProjeTakip.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeTakip.Models.ViewModels
{
    public class AdminDashboardVM
    {
        public int ActiveProjectCount { get; set; }
        public int CompletedProjectCount { get; set; }
        public int OngoingProjectCount { get; set; }

        public int TaskCount { get; set; }
        public int ActiveTaskCount { get; set; }
        public int CompletedTaskCount { get; set; }
        public int InProgressTaskCount { get; set; }
        public int IncompleteTaskCount { get; set; }

        public int ActiveTeamCount { get; set; } // Aktif ekip sayısı

        public List<Project> ActiveProjects { get; set; }
        public List<Görev> ActiveTasks { get; set; }

        public List<CalendarEvent> CalendarEvents { get; set; }

        // Yeni eklenen özellikler
        public int PendingProjectCount { get; set; }
        public string AdminName { get; set; }

        // Yeni eklemeler:
        public List<ProjectDueDateInfo> ProjectDueDateInfos { get; set; }
        public List<TaskDueDateInfo> TaskDueDateInfos { get; set; }
        public List<string> ProjectMessages { get; set; }
        public List<MessageWithColors> ProjectMessagesWithColor { get; set; }
    }

    public class ProjectDueDateInfo
    {
        public string ProjectName { get; set; }
        public int DaysUntilDueDate { get; set; }
    }

    public class TaskDueDateInfo
    {
        public string TaskName { get; set; }
        public int DaysUntilDueDate { get; set; }
    }
}
