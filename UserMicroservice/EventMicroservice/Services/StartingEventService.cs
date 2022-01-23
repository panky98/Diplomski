using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Models.EventMicroservice;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventMicroservice.Services
{
    public class StartingEventService : BackgroundService
    {
        public StartingEventService(DatabaseClient databaseClient,
                                ILogger<StartingEventService> logger)
        {
            this._databaseClient = databaseClient;
            this._logger = logger;
        }

        private readonly DatabaseClient _databaseClient;
        private readonly ILogger<StartingEventService> _logger;
        private Timer _timer;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this._logger.LogInformation("StartingEventService has started!");

            this._timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            DateTime dateTimeNow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(
                            DateTime.UtcNow, "Europe/Belgrade");

            _logger.LogInformation(dateTimeNow.ToString() + " - StartingEventServiceIsExecuting!");

            var collection = _databaseClient.MongoDatabase.GetCollection<Event>("events-collection");

            var filter = Builders<Event>.Filter.Where(x => true);


            var list = await collection.Find<Event>(filter).ToListAsync();

            list = list.Where(x => x.DateTimeOfEvent.Minute == dateTimeNow.Minute && x.DateTimeOfEvent.Hour == dateTimeNow.Hour && x.DateTimeOfEvent.Day == dateTimeNow.Day
                                                                                  && x.DateTimeOfEvent.Month == dateTimeNow.Month && x.DateTimeOfEvent.Year == dateTimeNow.Year).ToList();
            foreach (var eventStarting in list)
            {
                var newEventStarted = new EventDTO()
                {
                    Name = eventStarting.Name,
                    UserIds = eventStarting.userIds,
                    Code = eventStarting.Code
                };

                var config = new ProducerConfig
                {
                    BootstrapServers = "broker:9092",
                    MessageCopyMaxBytes = 15728640,
                    ReceiveMessageMaxBytes = 15728640,
                    MessageMaxBytes = 15728640,
                };

                var schemaRegistryConfig = new SchemaRegistryConfig
                {
                    // Note: you can specify more than one schema registry url using the
                    // schema.registry.url property for redundancy (comma separated list). 
                    // The property name is not plural to follow the convention set by
                    // the Java implementation.
                    Url = "schema_registry:8081"
                };

                using (var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig))
                using (var producer = new ProducerBuilder<Null, EventDTO>(config)
                            .SetValueSerializer(new JsonSerializer<EventDTO>(schemaRegistry))
                            .Build())
                {
                    _logger.LogInformation($"Sending event-started event to the topic!");
                    await producer.ProduceAsync("event-started", new Message<Null, EventDTO> { Value = newEventStarted });
                    _logger.LogInformation($"Produced event-started event in the topic!");
                }
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            //dispose everything that should be disposed
            _logger.LogInformation("EventCreatedService is stopping executing!");
            _timer?.Change(Timeout.Infinite, 0);
            _timer?.Dispose();
            return base.StopAsync(cancellationToken);
        }
    }
}
