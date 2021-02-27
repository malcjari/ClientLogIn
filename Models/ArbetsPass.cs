using ClientLogIn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models
{
    public class ArbetsPass
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int Shift { get; set; }
        public string Task { get; set; }

        public ArbetsPass(int id, DateTime date, int shift, string task)
        {
            this.Id = id;
            this.Date = date;
            this.Shift = shift;
            this.Task = task;
        }

        public ArbetsPass()
        {

        }

    }
}
