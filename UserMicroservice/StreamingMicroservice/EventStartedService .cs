using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Microsoft.Extensions.Logging;
using Models.EventMicroservice;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace StreamingMicroservice
{
    public class EventStartedService : Microsoft.Extensions.Hosting.BackgroundService
    {
        private Confluent.Kafka.IConsumer<Confluent.Kafka.Null, Models.EventMicroservice.EventDTO> _consumer;

        public EventStartedService(ILogger<EventStartedService> logger,
                                   IHttpClientFactory httpClientFactory)
        {
            this._logger = logger;
            this._httpClientFactory = httpClientFactory;
        }

        private readonly ILogger<EventStartedService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("EventStartedService has started executing!");

            var config = new ConsumerConfig
            {
                BootstrapServers = "broker:9092",
                GroupId = "streaminGroup",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                FetchMaxBytes = 15728640,
                MessageCopyMaxBytes = 15728640,
                ReceiveMessageMaxBytes = 15728640+513,
                MessageMaxBytes = 15728640
            };

            this._consumer = new Confluent.Kafka.ConsumerBuilder<Confluent.Kafka.Null, Models.EventMicroservice.EventDTO>(config)
                                    .SetValueDeserializer(new Confluent.SchemaRegistry.Serdes.JsonDeserializer<EventDTO>().AsSyncOverAsync())
                                    .Build();

            _logger.LogInformation("EventStartedService is trying to connect to topics!");
            this._consumer.Subscribe("event-started");

            while (!stoppingToken.IsCancellationRequested && (this._consumer.Subscription == null || this._consumer.Subscription.Count < 1))
            {
                Task.Delay(500);
                this._consumer.Subscribe("event-started");
                _logger.LogInformation("EventStartedService is trying to connect to topics!");
            }

            _logger.LogInformation("EventStartedService has subscribed to topics!");

#pragma warning disable 
            new Thread(async () => StartConsumerLoop(stoppingToken)).Start();
#pragma warning restore

            return Task.CompletedTask;
        }

        private async Task StartConsumerLoop(CancellationToken cancellationToken)
        {
            _logger.LogInformation("EventStartedService has started infinite loop!");
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var cr = this._consumer.Consume(cancellationToken);
                    this._logger.LogInformation($"Received from {cr.Topic}: {cr.Message.Key}: {cr.Message.Value.Name}");
                    var receivedMessage = cr.Message.Value;
                    this._logger.LogInformation($"Event with code {receivedMessage.Code} has just started!");

                    //after received event-started get bytes of the event and persist them in the cache for streaming
                    //after stream ends - remove from cache
                    var httpClient = _httpClientFactory.CreateClient();
                    this._logger.LogInformation($"Trying to get bytes of the event with code {receivedMessage.Code}");
                    var fileBytes = await httpClient.GetByteArrayAsync($"http://eventmicroservice/api/Events/{receivedMessage.Code}",cancellationToken);
                                   
                    RedisClient redis = new RedisClient("redis-cache", 6379);
                    this._logger.LogInformation($"Bytes persisted for the event with {receivedMessage.Code}, amount of bytes: {fileBytes.Count()}");
                    redis.Add<byte[]>(receivedMessage.Code, fileBytes);

                    //collect when event should expire
                    this._logger.LogInformation($"Collecting expiration DateTime for event with code {receivedMessage.Code}");
                    var response = await httpClient.GetAsync("http://eventmicroservice/api/Events/{receivedMessage.Code}/Check",cancellationToken);

                    var stream = response.Content.ReadAsStream();
                    var dateTimeOfEvent=await JsonSerializer.DeserializeAsync<DateTime>(stream, cancellationToken: cancellationToken);
                    this._logger.LogInformation($"Expiration DateTime for event with code {receivedMessage.Code} is {dateTimeOfEvent.ToString()}");

                    redis.Add<string>($"EXP_{receivedMessage.Code}", dateTimeOfEvent.ToString());
                    this._logger.LogInformation($"Expiration DateTime for event with code {receivedMessage.Code} persisted into cache");
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ConsumeException e)
                {
                    // Consumer errors should generally be ignored (or logged) unless fatal.
                    this._logger.LogError($"Consume error: {e.Error.Reason}");
                    this._logger.LogError($"Consume error: {e.Error.Code}");
                    this._logger.LogError($"Consume error: {e.Error.ToString()}");
                    this._logger.LogError($"Consume error: {e.InnerException}");
                    this._logger.LogError($"Consume error: {e.Message}");

                    if (e.Error.IsFatal)
                    {
                        // https://github.com/edenhill/librdkafka/blob/master/INTRODUCTION.md#fatal-consumer-errors
                        break;
                    }
                }
                catch (Exception e)
                {
                    this._logger.LogError($"Unexpected error: {e}");
                    break;
                }
            }
            _logger.LogInformation("CancelationToken requested for EventStartedService!");
        }
    }
}
