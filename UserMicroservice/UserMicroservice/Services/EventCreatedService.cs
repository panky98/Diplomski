using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Models.EventMicroservice;
using Confluent.SchemaRegistry.Serdes;

namespace UserMicroservice.Services
{
    public class EventCreatedService : BackgroundService
    {

        public EventCreatedService(ILogger<EventCreatedService> logger)
        {
            _logger = logger;
        }

        private readonly ILogger<EventCreatedService> _logger;
        private IConsumer<Null,Event> _consumer;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("EventCreatedService has started executing!");

            var config = new ConsumerConfig
            {
                BootstrapServers = "broker:9092",
                GroupId = "myGroup",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            this._consumer = new ConsumerBuilder<Null, Event>(config)
                                    .SetValueDeserializer(new JsonDeserializer<Event>().AsSyncOverAsync())
                                    .Build();

            _logger.LogInformation("EventCreatedService is trying to connect to event-created topic!");
            this._consumer.Subscribe("event-created");

            while (!stoppingToken.IsCancellationRequested && (this._consumer.Subscription == null || this._consumer.Subscription.Count==0))
            {
                Task.Delay(500);
                this._consumer.Subscribe("event-created");
                _logger.LogInformation("EventCreatedService is trying to connect to event-created topic!");
            }

            _logger.LogInformation("EventCreatedService has subscribed to event-created topic!");
            new Thread(() => StartConsumerLoop(stoppingToken)).Start();

            return Task.CompletedTask;
        }

        private void StartConsumerLoop(CancellationToken cancellationToken)
        {
            _logger.LogInformation("EventCreatedService has started infinite loop!");
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var cr = this._consumer.Consume(cancellationToken);

                    // Handle message...

                    this._logger.LogInformation($"Received: {cr.Message.Key}: {cr.Message.Value.Code}ms");
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


        //Executed when application is shuting down, to dispose everything what is needed to be disposed
        //main app will wait for finite amount of time on StopAsync (5seconds)
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            //dispose everything that should be disposed
            _logger.LogInformation("EventCreatedService is stopping executing!");
            this._consumer?.Close();
            this?._consumer.Dispose();
            return base.StopAsync(cancellationToken);
        }
    }
}
