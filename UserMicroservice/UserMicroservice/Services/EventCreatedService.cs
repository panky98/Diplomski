using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace UserMicroservice.Services
{
    public class EventCreatedService : BackgroundService
    {

        public EventCreatedService(ILogger<EventCreatedService> logger)
        {
            _logger = logger;
        }

        private readonly ILogger<EventCreatedService> _logger;
        private IConsumer<Ignore,string> _consumer;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("EventCreatedService has started executing!");

            var config = new ConsumerConfig
            {
                BootstrapServers = "broker:9092",
                GroupId = "myGroup",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            this._consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            this._consumer.Subscribe("event-created");
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
                    this._logger.LogInformation($"{cr.Message.Key}: {cr.Message.Value}ms");
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ConsumeException e)
                {
                    // Consumer errors should generally be ignored (or logged) unless fatal.
                    this._logger.LogError($"Consume error: {e.Error.Reason}");

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
