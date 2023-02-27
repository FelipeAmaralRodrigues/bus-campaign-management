using CampaignManagement.Contracts;
using MassTransit;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CampaignManagement.Consumer
{
    public class SmsMessageRejectedConsumer : IConsumer<SmsMessageRejected>
    {
        public async Task Consume(ConsumeContext<SmsMessageRejected> context)
        {
            var m = $"Message rejected {DateTime.UtcNow}: {{ uid: \"{context.Message.UId}\", message: \"{context.Message.Message}\", reason: \"{context.Message.Reason}\"}}";
            LogContext.Warning?.Log(m);
        }
    }
}
