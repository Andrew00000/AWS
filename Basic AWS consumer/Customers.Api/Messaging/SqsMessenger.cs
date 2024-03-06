using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Customers.Api.Messaging;

public class SqsMessenger: ISqsMessenger
{
    private readonly IAmazonSQS sqs;
    private readonly IOptions<QueueSettings> queueSettings;
    private string? queueUrl;

    public SqsMessenger(IAmazonSQS sqs, IOptions<QueueSettings> queueSettings)
    {
        this.sqs = sqs;
        this.queueSettings = queueSettings;
    }

    public async Task<SendMessageResponse> SendMessageAsync<T>(T message)
    {
        var queueUrl = await GetQueueUrlAsync();

        var sendMessageRequest = new SendMessageRequest
        {
            QueueUrl = queueUrl,
            MessageBody = JsonSerializer.Serialize(message),
            MessageAttributes = new Dictionary<string, MessageAttributeValue>
            {
                {
                    "MessageType", new MessageAttributeValue
                    {
                        DataType = "String",
                        StringValue = typeof(T).Name
                    }
                }
            }
        };

        return await sqs.SendMessageAsync(sendMessageRequest);
    }

    private async Task<string> GetQueueUrlAsync()
    {
        if (queueUrl is not null)
        {
            return queueUrl;
        }

        var queueUrlResponse = await sqs.GetQueueUrlAsync(queueSettings.Value.Name);
        queueUrl = queueUrlResponse.QueueUrl;
        return queueUrl;
    }
}
