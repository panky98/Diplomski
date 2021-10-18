using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Models.EventMicroservice;
using Microsoft.AspNetCore.SignalR.Client;

namespace StreamingMicroservice
{
    public class EventStartedService : Microsoft.Extensions.Hosting.BackgroundService
    {
        private Confluent.Kafka.IConsumer<Confluent.Kafka.Null, Models.EventMicroservice.EventDTO> _consumer;

        public EventStartedService(ILogger<EventStartedService> logger)
        {
            this._logger = logger;
        }

        private readonly ILogger<EventStartedService> _logger;


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("EventStartedService has started executing!");

            var config = new Confluent.Kafka.ConsumerConfig
            {
                BootstrapServers = "broker:9092",
                GroupId = "myGroup",
                AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest
            };

            this._consumer = new Confluent.Kafka.ConsumerBuilder<Confluent.Kafka.Null, Models.EventMicroservice.EventDTO>(config)
                                    .SetValueDeserializer(new Confluent.SchemaRegistry.Serdes.JsonDeserializer<EventDTO>().AsSyncOverAsync())
                                    .Build();

            _logger.LogInformation("EventStartedService is trying to connect to topics!");
            this._consumer.Subscribe(new List<string>() { "event-started" });

            while (!stoppingToken.IsCancellationRequested && (this._consumer.Subscription == null || this._consumer.Subscription.Count < 1))
            {
                Task.Delay(500);
                this._consumer.Subscribe(new List<string>() { "event-started" });
                _logger.LogInformation("EventStartedService is trying to connect to topics!");
            }

            _logger.LogInformation("EventStartedService has subscribed to topics!");
            new Thread(() => StartConsumerLoop(stoppingToken)).Start();

            return Task.CompletedTask;
        }

        private void StartConsumerLoop(CancellationToken cancellationToken)
        {
            _logger.LogInformation("EventStartedService has started infinite loop!");
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var cr = this._consumer.Consume(cancellationToken);                    
                    this._logger.LogInformation($"Received from {cr.Topic}: {cr.Message.Key}: {cr.Message.Value.Name}");

                    var task = new Task(delegate { TaskMethod(cr.Message.Value.Code, cr.Message.Value.Base64); });
                    task.Start();
                    
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
        }

        private async Task TaskMethod(string code,string base64)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:52802/streaming").Build();
            await connection.StartAsync();
            await connection.SendAsync("SendByte", ReadByte(Convert.FromBase64String(base64)),code);
        }

        private async IAsyncEnumerable<byte> ReadByte(byte[] array)
        {
            foreach(var el in array)
            {
                yield return el;
            }
        }
    }
}
