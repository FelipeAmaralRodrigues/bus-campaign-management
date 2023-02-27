using Azure.Messaging.ServiceBus;
using MassTransit;
using Microsoft.Azure.WebJobs;
using System.Threading;
using System.Threading.Tasks;

namespace CampaignManagement.Consumer.Functions
{
    public class SmsMessageReceivedFunction
    {
        readonly IMessageReceiver _receiver;

        public SmsMessageReceivedFunction(IMessageReceiver receiver)
        {
            _receiver = receiver;
        }

        [FunctionName("SmsMessageReceived")]
        public Task SmsMessageReceivedAsync([ServiceBusTrigger("campaignmanagement.contracts/smsmessagereceived", "fcn-sms-message-received")] ServiceBusReceivedMessage message, CancellationToken cancellationToken)
        {
            return _receiver.HandleConsumer<SmsMessageReceivedConsumer>("campaignmanagement.contracts/smsmessagereceived", "fcn-sms-message-received", message, cancellationToken);
        }
    }
}
