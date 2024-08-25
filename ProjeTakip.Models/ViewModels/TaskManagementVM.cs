using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeTakip.Models.ViewModels
{
    public class TaskManagementVM
    {
        public IEnumerable<Görev> Tasks { get; set; }
    }
}
