using CMS2.Models;
using Microsoft.EntityFrameworkCore;

namespace CMS2.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<CMS2.Models.User> Users { get; set; }

        public DbSet<TaskModel> Tasks { get; set; }

        public DbSet<EmailLog> EmailLogs { get; set; }

        public DbSet<TaskHistory> TaskHistory { get; set; } 


    }
}
