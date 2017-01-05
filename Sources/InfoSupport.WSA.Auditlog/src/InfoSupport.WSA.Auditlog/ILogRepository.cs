using InfoSupport.WSA.Logging.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoSupport.WSA.Logging
{
    public interface ILogRepository
    {
        IEnumerable<LogEntry> FindEntriesBy(LogEntryCriteria criteria);
        void AddEntry(LogEntry entry);
    }
}
