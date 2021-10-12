using EventMicroservice.Services;
using Microsoft.AspNetCore.Mvc;
using Models.EventMicroservice;
using System.Threading.Tasks;
using MongoDB.Driver;
using Confluent.Kafka;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using System;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;


namespace EventMicroservice.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : Controller
    {
        public EventsController(DatabaseClient databaseClient,
                                ILogger<EventsController> logger)
        {
            this._databaseClient = databaseClient;
            this._logger = logger;
        }

        private readonly DatabaseClient _databaseClient;
        private readonly ILogger<EventsController> _logger;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var collection=_databaseClient.MongoDatabase.GetCollection<Event>("events-collection");
            var retList=await collection.Find(x => true).ToListAsync();
            return Ok(retList);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOne([FromBody] EventDTO newEventFromBody)
        {

            _logger.LogInformation("Creating new event");
            var identity = (ClaimsIdentity)HttpContext.User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            Claim idClaim = Enumerable.ElementAt<Claim>(claims, 2);


            var newEvent = new Event()
            {
                CreatorId = Int32.Parse(idClaim.Value),
                Name = newEventFromBody.Name,
                InterestIds = newEventFromBody.InterestIds,
                DateTimeOfEvent = newEventFromBody.DateTimeOfEvent,
                Video = Convert.FromBase64String(newEventFromBody.Base64),
                userIds = new List<int>()
            };

            newEvent.userIds.Add(Int32.Parse(idClaim.Value));

            newEvent.Code = GetHashString(newEvent.Name);
            newEventFromBody.Code = newEvent.Code;
            newEventFromBody.CreatorId = newEvent.CreatorId;
            _logger.LogInformation($"New event's code: {newEvent.Code}");


            var collection = _databaseClient.MongoDatabase.GetCollection<Event>("events-collection");
            _logger.LogInformation($"Collection gathered, number of documents inside: {(collection!=null?collection.Find(x=>true).ToList().Count:-1)}");

            await collection.InsertOneAsync(newEvent);
            _logger.LogInformation($"Inserted new event!");

            var config = new ProducerConfig
            {
                BootstrapServers = "broker:9092"                
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
                _logger.LogInformation($"Sending event-created event to the topic!");
                await producer.ProduceAsync("event-created", new Message<Null, EventDTO> { Value = newEventFromBody });
                _logger.LogInformation($"Produced event-created event in the topic!");

            }

            return Ok(newEvent);
        }

        [HttpDelete]
        [Route("DeleteAll")]
        public async Task<IActionResult> DeleteAll()
        {
            var collection =await _databaseClient.MongoDatabase.GetCollection<Event>("events-collection").DeleteManyAsync(x => true);
            return Ok();
        }

        private byte[] GetHash(string inputString)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }
        private string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
    }
}
