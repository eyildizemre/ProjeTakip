using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeTakip.Models.ViewModels
{
    public class DeliverProjectVM
    {
        public int ProjectId { get; set; }
        [ValidateNever]
        public string ProjectName { get; set; }
        [ValidateNever]
        public string ProjectDescription { get; set; }
        [ValidateNever]
        public string TeamName { get; set; }
        [ValidateNever]
        public string TeamLeadName { get; set; }
        [ValidateNever]
        public DateTime StartDate { get; set; }
        [ValidateNever]
        public DateTime EndDate { get; set; }
        [ValidateNever]
        public int ProjectStatusId { get; set; }
        [ValidateNever]
        public string ProjectStatusName { get; set; }
        public string GitHubPush { get; set; }
    }
}
