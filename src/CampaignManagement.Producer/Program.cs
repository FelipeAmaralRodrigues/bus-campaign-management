using CampaignManagement.Contracts;
using CampaignManagement.Producer.HostedServices;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", false, true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true, true)
    .AddCommandLine(args)
    .AddEnvironmentVariables()
    .Build();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(builder =>
    {
        builder.Sources.Clear();
        builder.AddConfiguration(configuration);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            x.AddServiceBusMessageScheduler();

            x.UsingAzureServiceBus((context, cfg) =>
            {
                var connectionString = configuration.GetConnectionString("BusConnection");
                cfg.Host(connectionString);
                cfg.ConfigureEndpoints(context);
            });

            x.AddRequestClient<SubmitSmsMessage>(new Uri("queue:submit-sms-message"));
        });

        services.AddHostedService<Worker>();
    });

await host.StartAsync();

while (true);
