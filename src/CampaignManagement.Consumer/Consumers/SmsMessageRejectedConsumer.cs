using CampaignManagement.Contracts;
using MassTransit;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Threading.Tasks;
using System.Text.Json;

namespace CampaignManagement.Consumer
{
    public class SmsMessageRejectedConsumer : IConsumer<SmsMessageRejected>
    {
        public async Task Consume(ConsumeContext<SmsMessageRejected> context)
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process));
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                CloudTable table = tableClient.GetTableReference("TwilioErrors");
                await table.CreateIfNotExistsAsync();
                TableOperation insertOperation = TableOperation.Insert(new SmsMessageRejectedTableInsert(
                    context.Message.ToPhoneNumber, 
                    $"{context.Message.Reason.Replace(" ", "-").ToLower()} [{context.Message.UId}]") { SmsMessageJson = JsonSerializer.Serialize(context.Message) });
                await table.ExecuteAsync(insertOperation);

                var m = $"Message rejected {DateTime.UtcNow}: {{ uid: \"{context.Message.UId}\", message: \"{context.Message.Message}\", reason: \"{context.Message.Reason}\"}}";
                LogContext.Warning?.Log(m);
            }
            catch (StorageException e)
            {
                throw;
            }

        }
    }


    public class SmsMessageRejectedTableInsert : TableEntity 
    {
        public SmsMessageRejectedTableInsert(string skey, string srow)
        {
            this.PartitionKey = skey;
            this.RowKey = srow;
        }

        public string SmsMessageJson { get; set; }
    }

    public class SmsMessageRejectedConsumerDefinition : ConsumerDefinition<SmsMessageRejectedConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<SmsMessageRejectedConsumer> consumerConfigurator)
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
