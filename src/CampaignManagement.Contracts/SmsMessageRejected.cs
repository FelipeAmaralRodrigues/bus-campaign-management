using System;

namespace CampaignManagement.Contracts
{
    public interface SmsMessageRejected
    {
        public Guid UId { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; }
        public string Reason { get; set; }
    }
}
