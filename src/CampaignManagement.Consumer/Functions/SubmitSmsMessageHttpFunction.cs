using CampaignManagement.Contracts;
using MassTransit;
using MassTransit.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;

namespace CampaignManagement.Consumer
{
    public class SubmitSmsMessageHttpFunction
    {
        readonly IRequestClient<SubmitSmsMessage> _client;
        readonly IAsyncBusHandle _handle;

        public SubmitSmsMessageHttpFunction(IAsyncBusHandle handle, IRequestClient<SubmitSmsMessage> client)
        {
            _handle = handle;
            _client = client;
        }

        [FunctionName("sms-message")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest request, ILogger logger)
        {
            var body = await request.ReadAsStringAsync();

            var order = JsonSerializer.Deserialize<SubmitSmsMessage>(body, SystemTextJsonMessageSerializer.Options);
            if (order == null)
                return new BadRequestResult();

            logger.LogInformation("SubmitOrder HTTP Function: {UId} -> {Message}", order.UId, order.Message);

            var response = await _client.GetResponse<SmsMessageAccepted>(order);

            return new OkObjectResult(new
            {
                response.Message.UId
            });
        }
    }
}
