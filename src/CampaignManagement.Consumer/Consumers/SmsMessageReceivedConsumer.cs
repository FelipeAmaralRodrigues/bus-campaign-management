using CampaignManagement.Contracts;
using MassTransit;
using MassTransit.AzureServiceBusTransport;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Twilio.TwiML.Messaging;

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

    public class SmsMessageReceivedConsumerDefinition : ConsumerDefinition<SmsMessageReceivedConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<SmsMessageReceivedConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseDelayedRedelivery(r =>
            {
                r.Intervals(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(30));
            });

            consumerConfigurator.UseMessageRetry(x =>
            {
                x.Immediate(2);
            });

            endpointConfigurator.UseInMemoryOutbox();
        }
    }
}
