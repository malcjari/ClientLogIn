using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientLogIn.Models
{
    public class WorkShift
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int UserId  { get; set; }
        public User User { get; set; }
        public int TaskId { get; set; }
        public Task Task { get; set; }
        public int ShiftTypeId { get; set; }
        public ShiftType ShiftType { get; set; }
        public WorkShift(int id, DateTime date, int shift, string task)
        {
            this.Id = id;
            this.Date = date;
            this.ShiftTypeId = shift;
            this.TaskId = task;
        }

        public ArbetsPass()
        {

        }
    }

   
}
