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
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using ServiceStack.Redis;

namespace UserMicroservice.Services
{
    public class EventService : BackgroundService
    {

        public EventService(ILogger<EventService> logger,
                                   IHubContext<NotificationsHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;            
        }

        private readonly ILogger<EventService> _logger;
        private IConsumer<Null, EventDTO> _consumer;
        private readonly IHubContext<NotificationsHub> _hubContext;
        readonly RedisClient _redisClient = new RedisClient("redis-api", 6379);


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("EventService has started executing!");

            var config = new ConsumerConfig
            {
                BootstrapServers = "broker:9092",
                GroupId = "userGroup",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                FetchMaxBytes= 15728640,
                MessageCopyMaxBytes= 15728640,
                ReceiveMessageMaxBytes= 15728640+513,
                MessageMaxBytes= 15728640                
            };

            this._consumer = new ConsumerBuilder<Null, EventDTO>(config)
                                    .SetValueDeserializer(new JsonDeserializer<EventDTO>().AsSyncOverAsync())
                                    .Build();

            _logger.LogInformation("EventService is trying to connect to topics!");
            this._consumer.Subscribe(new List<string>(){ "event-created","event-started"});

            while (!stoppingToken.IsCancellationRequested && (this._consumer.Subscription == null || this._consumer.Subscription.Count<2))
            {
                Task.Delay(500);
                this._consumer.Subscribe(new List<string>() { "event-created", "event-started" });
                _logger.LogInformation("EventService is trying to connect to topics!");
            }

            _logger.LogInformation("EventService has subscribed to topics!");
            new Thread(() => StartConsumerLoop(stoppingToken)).Start();

            return Task.CompletedTask;
        }

        private void StartConsumerLoop(CancellationToken cancellationToken)
        {
            _logger.LogInformation("EventService has started infinite loop!");
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var cr = this._consumer.Consume(cancellationToken);
                    if(cr.Topic=="event-created")
                    {
                        this._logger.LogInformation($"Received from {cr.Topic}: {cr.Message.Key}: {cr.Message.Value.Name}");
                        this._logger.LogInformation($"Connected status to redis: {_redisClient.HasConnected}");

                        string creatorConnectionId = string.Empty;
                        if (_redisClient.HashContainsEntry("allActiveUsers", cr.Message.Value.CreatorId.ToString()))
                        {
                            creatorConnectionId = _redisClient.GetValueFromHash("allActiveUsers", cr.Message.Value.CreatorId.ToString());
                        }
                        this._logger.LogInformation($"Connected status to redis: {_redisClient.HasConnected} - creatorOfEventId: {creatorConnectionId}");


                        //send to all users which are active and which are interested in event's interests(categories)
                        IList<string> coveredContextIds = new List<string>();
                        _logger.LogInformation($"Sending notifications because event is created to active users for {cr.Message.Value.InterestIds.Count} different interest categories");
                        foreach (var interestId in cr.Message.Value.InterestIds)
                        {
                            IList<string> members = _redisClient.GetAllItemsFromList("interest:" + interestId);
                            _logger.LogInformation($"Gathered connectionIds for interest with id {interestId} amount:{members?.Count} ");

                            if (members != null)
                            {
                                foreach (var member in members)
                                {
                                    if (!coveredContextIds.Contains(member) && member != creatorConnectionId)
                                    {
                                        coveredContextIds.Add(member);
                                        _hubContext.Clients.Client(member).SendAsync("EventCreatedNotification", cr.Message.Value);
                                        _logger.LogInformation($"Sended notification about created event to user with ContextId: {member}");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        this._logger.LogInformation($"Received from {cr.Topic}: {cr.Message.Key}: {cr.Message.Value.Name}");
                        this._logger.LogInformation($"Connected status to redis: {_redisClient.HasConnected}");
                        foreach(var userId in cr.Message.Value.UserIds)
                        {
                            if (_redisClient.HashContainsEntry("allActiveUsers", userId.ToString()))
                            {
                                var userIdConnectionId = _redisClient.GetValueFromHash("allActiveUsers", userId.ToString());
                                _hubContext.Clients.Client(userIdConnectionId).SendAsync("EventStartedNotification", $"Event for which you are subscribed({cr.Message.Value.Name}) has started!");
                                _logger.LogInformation($"Sended notification about started event to user with ContextId: {userIdConnectionId} dbId{userId} about event with name{cr.Message.Value.Name}");
                            }
                        }
                    }
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
            _logger.LogInformation("EventService is stopping executing!");
            this._consumer?.Close();
            this?._consumer.Dispose();
            return base.StopAsync(cancellationToken);
        }
    }
}
