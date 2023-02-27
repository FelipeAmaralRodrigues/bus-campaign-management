using CampaignManagement.Contracts;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CampaignManagement.Producer.HostedServices
{
    public class Worker : IHostedService, IDisposable
    {
        private readonly IBus _bus;
        private readonly IRequestClient<SubmitSmsMessage> _client;
        private readonly IMessageScheduler _messageScheduler;
        private ILogger<Worker> _logger;
        
        private int _executionCount = 0;

        public Worker(IBus bus, IRequestClient<SubmitSmsMessage> client, ILogger<Worker> logger, IMessageScheduler messageScheduler)
        {
            _bus = bus;
            _client = client;
            _logger = logger;
            _messageScheduler = messageScheduler;
        }

        public void Dispose()
        {
            Dispose();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(3));
            while (!cancellationToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken))
            {
                try
                {
                    var command = new SubmitSmsMessage { UId = Guid.NewGuid(), Message = $"Message {_executionCount} " + DateTime.UtcNow.ToString() };
                    _executionCount++;
                    _logger.LogInformation($"Message sended: {{ uid: \"{command.UId}\", message: \"{command.Message}\"}}");

                    // send
                    //var endpoint = await _bus.GetSendEndpoint(new Uri("queue:submit-sms-message"));
                    //await endpoint.Send(command);

                    // request
                    //Response<SmsMessageAccepted> e = await _client.GetResponse<SmsMessageAccepted>(new SubmitSmsMessage { UId = Guid.NewGuid(), Message = "Teste" });

                    // send scheduler
                    await _messageScheduler.ScheduleSend<SubmitSmsMessage>(new Uri("queue:submit-sms-message"), DateTime.UtcNow + TimeSpan.FromSeconds(30), command);
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(
                        $"Failed to execute PeriodicHostedService with exception message {ex.Message}. Good luck next round!");
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            return Task.CompletedTask;
        }
    }
}
