using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientLogIn.Models
{
    public class DateDataModel
    {
        public int Days { get; set; }
        public int FirstDayOfWeek { get; set; }

        public int TodaysDate { get; set; }
        public int Month { get; set; }

        //Datumet som används vid ny månad
        public DateTime FullDate { get; set; }
        
        //Datumet som används vid click-events
        public DateTime ClickedFullDate { get; set; }
        public string MonthName { get; set; }
    }
}
