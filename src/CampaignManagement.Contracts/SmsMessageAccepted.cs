using System;

namespace CampaignManagement.Contracts
{
    public interface SmsMessageAccepted
    {
        public Guid UId { get; set; }
        public string Message { get; set; }
    }
}
