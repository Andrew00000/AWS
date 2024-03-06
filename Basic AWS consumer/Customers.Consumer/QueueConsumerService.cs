using Amazon.SQS;
using Amazon.SQS.Model;
using Customers.Consumer.Messages;
using MediatR;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Customers.Consumer;

public class QueueConsumerService : BackgroundService
{
    private readonly IAmazonSQS sqs;
    private readonly IOptions<QueueSettings> queueSettings;
    private readonly IMediator mediator;
    private readonly ILogger<QueueConsumerService> logger;

    public QueueConsumerService(IAmazonSQS sqs, IOptions<QueueSettings> queueSettings, IMediator mediator, ILogger<QueueConsumerService> logger)
    {
        this.sqs = sqs;
        this.queueSettings = queueSettings;
        this.mediator = mediator;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var queueUrlResponse = await sqs.GetQueueUrlAsync(queueSettings.Value.Name, cancellationToken);

        var receiveMessageRequest = new ReceiveMessageRequest
        {
            QueueUrl = queueUrlResponse.QueueUrl,
            AttributeNames = new List<string> { "All" },
            MessageAttributeNames = new List<string> { "All" },
            MaxNumberOfMessages = 1
        };

        while (!cancellationToken.IsCancellationRequested)
        {
            var response = await sqs.ReceiveMessageAsync(receiveMessageRequest, cancellationToken);
            foreach (var message in response.Messages)
            {
                var messageType = message.MessageAttributes["MessageType"].StringValue;

                var type = Type.GetType($"Customers.Consumer.Messages.{messageType}");
                if(type is null)
                {
                    logger.LogWarning("Unknown message type: {messageType}", messageType);
                    continue;
                }
                
                var typedMessage = (ISqsMessage)JsonSerializer.Deserialize(message.Body, type)!;

                try
                {
                    await mediator.Send(typedMessage,cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Message failed during processing");
                    continue;
                }

                await sqs.DeleteMessageAsync(queueUrlResponse.QueueUrl, message.ReceiptHandle, cancellationToken);
            }

            await Task.Delay(1000, cancellationToken);
        }
    }
}
