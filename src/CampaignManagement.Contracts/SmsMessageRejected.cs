using System;

namespace CampaignManagement.Contracts
{
    public record SmsMessageRejected
    {
        public Guid UId { get; set; }
        public string ToPhoneNumber { get; set; }
        public string FromPhoneNumber { get; set; }
        public string Message { get; set; }
        public string UrlImage { get; set; }
        public string SubAccountSID { get; set; }

        public DateTime Timestamp { get; }
        public string Reason { get; set; }
    }
}
