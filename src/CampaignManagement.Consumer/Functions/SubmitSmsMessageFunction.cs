using Azure.Messaging.ServiceBus;
using MassTransit;
using Microsoft.Azure.WebJobs;
using System.Threading;
using System.Threading.Tasks;

namespace CampaignManagement.Consumer
{
    public class SubmitSmsMessageFunction
    {
        const string SubmitSmsMessageQueueName = "submit-sms-message";
        readonly IMessageReceiver _receiver;

        public SubmitSmsMessageFunction(IMessageReceiver receiver)
        {
            _receiver = receiver;
        }

        [FunctionName("SubmitSmsMessage")]
        public Task SubmitSmsMessageAsync([ServiceBusTrigger(SubmitSmsMessageQueueName)] ServiceBusReceivedMessage message, CancellationToken cancellationToken)
        {
            return _receiver.HandleConsumer<SubmitSmsMessageConsumer>(SubmitSmsMessageQueueName, message, cancellationToken);
        }
    }
}
