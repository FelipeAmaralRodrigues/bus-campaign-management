using CampaignManagement.Contracts;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace CampaignManagement.Consumer
{
    public class SubmitSmsMessageConsumer : IConsumer<SubmitSmsMessage>
    {
        public async Task Consume(ConsumeContext<SubmitSmsMessage> context)
        {
            var username = Environment.GetEnvironmentVariable("TWILIO_USERNAME");
            var password = Environment.GetEnvironmentVariable("TWILIO_PASSWORD");
            var urlCallBack = Environment.GetEnvironmentVariable("TWILIO_URLCALLBACK");

            TwilioClient.Init(username, password);

            var to = new PhoneNumber(context.Message.ToPhoneNumber);
            var from = new PhoneNumber(context.Message.FromPhoneNumber);

            List<Uri> images = new List<Uri>();
            if (!string.IsNullOrEmpty(context.Message.UrlImage))
            {
                var imageUri = new Uri(context.Message.UrlImage);
                images.Add(imageUri);
            }

            try
            {
                MessageResource result;
                result = await MessageResource.CreateAsync(
                    to: to,
                    from: from,
                    body: context.Message.Message,
                    mediaUrl: images,
                    statusCallback: new Uri(urlCallBack),
                    pathAccountSid: context.Message.SubAccountSID);


                await context.Publish<SmsMessageReceived>(new
                {
                    context.Message.UId,
                    context.Message.ToPhoneNumber,
                    context.Message.FromPhoneNumber,
                    context.Message.Message,
                    context.Message.UrlImage,

                    context.Message.SubAccountSID,
                    urlCallBack,

                    Timestamp = DateTime.UtcNow
                });

                LogContext.Debug?.Log($"Submit message {DateTime.UtcNow}: {{ uid: \"{context.Message.UId}\", message: \"{context.Message.Message}\"}}");
            }
            catch (ApiException ex)
            {
                await context.Publish<SmsMessageRejected>(new
                {
                    context.Message.UId,
                    context.Message.ToPhoneNumber,
                    context.Message.FromPhoneNumber,
                    context.Message.Message,
                    context.Message.UrlImage,

                    context.Message.SubAccountSID,
                    urlCallBack,

                    Timestamp = DateTime.UtcNow,
                    Reason = ex.Message
                });
            }

            if (context.IsResponseAccepted<SmsMessageAccepted>())
            {
                await context.RespondAsync<SmsMessageAccepted>(new {
                    context.Message.UId,
                    context.Message.ToPhoneNumber,
                    context.Message.FromPhoneNumber,
                    context.Message.Message,
                    context.Message.UrlImage,

                    context.Message.SubAccountSID,
                    urlCallBack,

                    Timestamp = DateTime.UtcNow,
                });
            }
        }
    }

    public class SubmitSmsMessageConsumerDefinition : ConsumerDefinition<SubmitSmsMessageConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<SubmitSmsMessageConsumer> consumerConfigurator)
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
