using CampaignManagement.Consumer.Functions;
using CampaignManagement.Contracts;
using MassTransit;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(CampaignManagement.Consumer.Startup))]
namespace CampaignManagement.Consumer
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services
                .AddScoped<SubmitSmsMessageFunction>()
                .AddScoped<SubmitSmsMessageHttpFunction>()
                .AddScoped<SmsMessageReceivedFunction>()
                .AddScoped<SmsMessageRejectedFunction>()
                .AddMassTransitForAzureFunctions(cfg =>
                {
                    cfg.AddConsumersFromNamespaceContaining<ConsumerNamespace>();
                    cfg.AddRequestClient<SubmitSmsMessage>(new Uri($"queue:submit-sms-message"));
                },
                "AzureWebJobsServiceBus");
        }
    }
}
