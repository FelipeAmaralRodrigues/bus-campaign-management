using MassTransit;

namespace CampaignManagement.Consumer.Consumers
{
    public class SubmitSmsMessageConsumerDefinition : ConsumerDefinition<SubmitSmsMessageConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<SubmitSmsMessageConsumer> consumerConfigurator)
        {
            consumerConfigurator.UseMessageRetry(x => x.Intervals(10, 100, 500, 1000));
        }
    }
}
