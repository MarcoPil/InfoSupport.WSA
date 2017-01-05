using InfoSupport.WSA.Logging.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace InfoSupport.WSA.Logging.DAL
{
    public class LoggerContext : DbContext
    {
        //public LoggerContext()
        //{
        //    Database.EnsureCreated();
        //}
        public LoggerContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<LogEntry> LogEntries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //var builder = new ConfigurationBuilder()
                //                    .SetBasePath(Directory.GetCurrentDirectory())
                //                    .AddJsonFile("appsettings.json");
                //var config = builder.Build();
                //string connectionString = config.GetConnectionString("AuditLogDatabase");
                string connectionString = "Server=.\\SQLEXPRESS;Database=AuditLogDB;Integrated Security=true";
                optionsBuilder.UseSqlServer(connectionString);
            }

            base.OnConfiguring(optionsBuilder);
        }
    }
}
