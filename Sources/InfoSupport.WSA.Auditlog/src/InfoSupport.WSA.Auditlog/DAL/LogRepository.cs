using System;
using System.Collections.Generic;
using InfoSupport.WSA.Logging;
using InfoSupport.WSA.Logging.DAL;
using InfoSupport.WSA.Logging.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.RegularExpressions;

namespace InfoSupport.WSA.Logging.DAL
{
    public class LogRepository : ILogRepository
    {
        private readonly DbContextOptions _options;

        public LogRepository(DbContextOptions options)
        {
            _options = options;
        }

        public void AddEntry(LogEntry entry)
        {
            using (var context = new LoggerContext(_options))
            {
                context.LogEntries.Add(entry);
                context.SaveChanges();
            }
        }

        public IEnumerable<LogEntry> FindEntriesBy(LogEntryCriteria criteria)
        {
            using (var context = new LoggerContext(_options))
            {
                IQueryable<LogEntry> result = context.LogEntries;

                result = result.Where(entry =>
                    (criteria.FromTimestamp == null || entry.Timestamp >= criteria.FromTimestamp) &&
                    (criteria.ToTimestamp == null   || entry.Timestamp <= criteria.ToTimestamp  ) &&
                    (criteria.EventType == null     || entry.EventType == criteria.EventType    )
                );

                if (criteria.RoutingKeyExpression != null)
                {
                    var pattern = criteria.RoutingKeyExpression
                                          .Replace(@".",@"\.")
                                          .Replace(@"*", @"[^.]*")
                                          .Replace(@"#", @".*");
                    pattern = "^" + pattern + "$";
                    Regex regex = new Regex(pattern);

                    result = result.Where(entry => regex.IsMatch(entry.RoutingKey));
                }
                return result.ToList();
            }
        }
    }
}