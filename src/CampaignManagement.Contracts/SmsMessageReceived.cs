using System;

namespace CampaignManagement.Contracts
{
    public interface SmsMessageReceived
    {
        public Guid UId { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; }
    }
}
