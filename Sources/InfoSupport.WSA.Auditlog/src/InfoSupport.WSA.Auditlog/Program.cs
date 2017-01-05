using InfoSupport.WSA.Infrastructure;
using InfoSupport.WSA.Logging.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InfoSupport.WSA.Logging
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("log: [AuditlogService] waiting 30 seconds for RabbitMQ and SQL Server to be ready...");
            Thread.Sleep(30000);
            DbContextOptions<LoggerContext> dbOptions = ReadDatabaseConfiguration();
            BusOptions busOptions = ReadBusConfiguration();

            var auditlogReplayService = new AuditlogReplayService(
                                new LogRepository(dbOptions),
                                new EventReplayer(busOptions));
            using (var auditlog = new AuditlogEventListener(new LogRepository(dbOptions), busOptions))
            using (var replayHost = new MicroserviceHost<AuditlogReplayService>(auditlogReplayService, busOptions))
            {
                auditlog.Start();
                Console.WriteLine("Auditlog is listening to Events...");
                replayHost.Open();
                Console.WriteLine("ReplayService is listening to Event Replay requests...");

                KeepApplicationAlive();
            }
            Console.WriteLine("Auditlog has stopped listening to Events...");
            Console.WriteLine("ReplayService has stopped listening to replay requests...");
        }

        private static BusOptions ReadBusConfiguration()
        {
            Console.WriteLine("Configuring Eventbus:");
            var busOptions = BusOptions.CreateFromEnvironment();
            Console.WriteLine(busOptions.ToString());
            return busOptions;
        }

        public static EventWaitHandle toStop = new AutoResetEvent(false);

        private static DbContextOptions<LoggerContext> ReadDatabaseConfiguration()
        {
            string connectionString = Environment.GetEnvironmentVariable("AuditLogDatabase");
            Console.WriteLine("log: [AuditlogService] read connectionString from Enviroment: {0}", connectionString);

            if (connectionString == null)
            {
                var config = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile("appsettings.json")
                                    .Build();

                connectionString = config.GetConnectionString("AuditLogDatabase");
            }

            var dbOptions = new DbContextOptionsBuilder<LoggerContext>()
                                .UseSqlServer(connectionString)
                                .Options;
            Console.WriteLine($"Configuring Auditlog Database: {connectionString}");
            return dbOptions;
        }

        private static void KeepApplicationAlive()
        {
            if (Environment.GetEnvironmentVariable("WSA_AUDITLOG_ENV") == "console")
            {
                Console.WriteLine("(Press any key to quit)");
                Console.ReadKey();
            }
            else
            {
                toStop.WaitOne();
            }
        }

        public static void Stop()
        {
            toStop.Set();
        }
    }
}
