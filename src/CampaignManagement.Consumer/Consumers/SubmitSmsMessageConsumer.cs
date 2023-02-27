using CampaignManagement.Contracts;
using MassTransit;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CampaignManagement.Consumer
{
    public class SubmitSmsMessageConsumer : IConsumer<SubmitSmsMessage>
    {
        public async Task Consume(ConsumeContext<SubmitSmsMessage> context)
        {
            LogContext.Debug?.Log($"Submit message {DateTime.UtcNow}: {{ uid: \"{context.Message.UId}\", message: \"{context.Message.Message}\"}}");

            if (context.Message.Message.Last() != '5')
                await context.Publish<SmsMessageReceived>(new
                {
                    context.Message.UId,
                    context.Message.Message,
                    Timestamp = DateTime.UtcNow
                });
            else
                await context.Publish<SmsMessageRejected>(new
                {
                    context.Message.UId,
                    context.Message.Message,
                    Timestamp = DateTime.UtcNow,
                    Reason = "Phone number invalid"
                });

            if (context.IsResponseAccepted<SmsMessageAccepted>())
            {
                await context.RespondAsync<SmsMessageAccepted>(context.Message);
            }
        }
    }
}
