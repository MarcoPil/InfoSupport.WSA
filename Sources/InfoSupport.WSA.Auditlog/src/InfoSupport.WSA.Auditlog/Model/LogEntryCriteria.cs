namespace InfoSupport.WSA.Logging.Model
{
    public class LogEntryCriteria
    {
        public long? FromTimestamp { get; set; }
        public long? ToTimestamp { get; set; }
        public string EventType { get; set; }
        public string RoutingKeyExpression { get; set; }
    }
}