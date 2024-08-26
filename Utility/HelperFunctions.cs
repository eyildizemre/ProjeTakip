using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeTakip.Utility
{
    public static class HelperFunctions
    {
        private static Random random = new Random();

        public static string GetRandomColor()
        {
            var colors = new List<string>
            {
                "#FF5733", "#33FF57", "#3357FF", "#F3FF33", "#FF33F3", "#33FFF3"
            };

            string color;
            do
            {
                color = colors[random.Next(colors.Count)];
            }
            while (color == "#FF5733"); // Kırmızı renk olmaz.

            return color;
        }
    }
}
