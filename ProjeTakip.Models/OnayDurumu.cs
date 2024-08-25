using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeTakip.Models
{
    public class OnayDurumu
    {
        [Key]
        public int OnayDurumuId { get; set; }
        [MaxLength(50)]
        [Display(Name = "Onay Durumu")]
        public string OnayDurumuAdi { get; set; }
    }
}
