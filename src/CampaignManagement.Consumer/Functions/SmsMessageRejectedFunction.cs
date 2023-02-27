using Azure.Messaging.ServiceBus;
using MassTransit;
using Microsoft.Azure.WebJobs;
using System.Threading;
using System.Threading.Tasks;

namespace CampaignManagement.Consumer.Functions
{
    public class SmsMessageRejectedFunction
    {
        readonly IMessageReceiver _receiver;

        public SmsMessageRejectedFunction(IMessageReceiver receiver)
        {
            _receiver = receiver;
        }

        [FunctionName("SmsMessageRejected")]
        public Task SmsMessageReceivedAsync([ServiceBusTrigger("campaignmanagement.contracts/smsmessagerejected", "fcn-sms-message-rejected")] ServiceBusReceivedMessage message, CancellationToken cancellationToken)
        {
            return _receiver.HandleConsumer<SmsMessageRejectedConsumer>("campaignmanagement.contracts/smsmessagerejected", "fcn-sms-message-rejected", message, cancellationToken);
        }
    }
}
