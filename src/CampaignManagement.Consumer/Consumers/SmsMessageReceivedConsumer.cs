using CampaignManagement.Contracts;
using MassTransit;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CampaignManagement.Consumer
{
    public class SmsMessageReceivedConsumer : IConsumer<SmsMessageReceived>
    {
        public async Task Consume(ConsumeContext<SmsMessageReceived> context)
        {
            var m = $"Message received {DateTime.UtcNow}: {{ uid: \"{context.Message.UId}\", message: \"{context.Message.Message}\"}}";
            LogContext.Warning?.Log(m);
        }
    }
}
