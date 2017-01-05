using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using InfoSupport.WSA.Logging.DAL;

namespace InfoSupport.WSA.Logging.Migrations
{
    [DbContext(typeof(LoggerContext))]
    [Migration("20161227204124_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("InfoSupport.WSA.Logging.Model.LogEntry", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("EventJson");

                    b.Property<string>("EventType");

                    b.Property<string>("RoutingKey");

                    b.Property<long>("Timestamp");

                    b.HasKey("ID");

                    b.ToTable("LogEntries");
                });
        }
    }
}
