using System;

namespace CampaignManagement.Contracts
{
    public record SubmitSmsMessage
    {
        public Guid UId { get; set; }
        public string Message { get; set; }
    }
}
