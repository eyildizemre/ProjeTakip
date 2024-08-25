using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjeTakip.Models;
using System.Collections.Generic;

namespace ProjeTakip.Models.ViewModels
{
    public class TeamVM
    {
        public Team Team { get; set; } = new Team();
        public IEnumerable<SelectListItem> TeamLeads { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Users { get; set; } = new List<SelectListItem>();
        public List<int> SelectedUserIds { get; set; } = new List<int>();

        // UserTeam modelini kullanarak ekibe ait kullanıcıların ve rollerinin listesini tutacak yeni alan
        public List<UserTeam> UserTeams { get; set; } = new List<UserTeam>();

        public TeamVM()
        {
            SelectedUserIds = new List<int>();
            UserTeams = new List<UserTeam>();
        }

        [ValidateNever]
        public List<CalendarEvent> CalendarEvents { get; set; }
    }
}