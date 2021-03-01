using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientLogIn.Models
{
    public class MyContext: IdentityDbContext<User, Role, int>
    {
        public DbSet<WorkShift> WorkShifts { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<ShiftType> ShiftTypes  { get; set; }

        public MyContext(DbContextOptions<MyContext> options): base(options) 
        {
            
        }
    }
}
