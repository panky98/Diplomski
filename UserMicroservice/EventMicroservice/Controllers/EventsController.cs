using EventMicroservice.Services;
using Microsoft.AspNetCore.Mvc;
using Models.EventMicroservice;
using System.Threading.Tasks;
using MongoDB.Driver;
using Confluent.Kafka;
using System.Security.Cryptography;
using System.Text;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace EventMicroservice.Controllers
{
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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Gathering all events");
            var identity = (ClaimsIdentity)HttpContext.User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            Claim idClaim = Enumerable.ElementAt<Claim>(claims, 2);
            int id = Int32.Parse(idClaim.Value);

            var collection=_databaseClient.MongoDatabase.GetCollection<Event>("events-collection");
            var retList=await collection.Find(x => true).ToListAsync();

            retList = retList.Where(x => x.userIds != null && !x.userIds.Contains(id)).ToList();
            DateTime dateTimeNow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(
                    DateTime.UtcNow, "Europe/Belgrade");
            retList = retList.Where(x => x.DateTimeOfEvent > dateTimeNow).ToList();

            return Ok(retList);
        }
        
        [HttpGet("{code}")]
        public async Task<IActionResult> GetByCode([FromRoute(Name ="code")] string code)
        {
            _logger.LogInformation($"Requesting bytes for the event with code {code}");            

            var collection = _databaseClient.MongoDatabase.GetCollection<Event>("events-collection");
            var retList = await collection.Find(x => x.Code==code).ToListAsync();

            var first = retList.FirstOrDefault();

            if (first == null)
            {
                _logger.LogInformation($"Event with code {code} does not exist");
                return NotFound();
            }

            return Ok(first.Video);
        }

        [HttpGet("{code}/Check")]
        public async Task<IActionResult> CheckEvent([FromRoute(Name = "code")] string code)
        {
            _logger.LogInformation($"Requesting check for the event with code {code}");

            var collection = _databaseClient.MongoDatabase.GetCollection<Event>("events-collection");
            var retList = await collection.Find(x => x.Code == code).ToListAsync();

            var first = retList.FirstOrDefault();

            if (first == null)
            {
                _logger.LogInformation($"Event with code {code} does not exist");
                return NotFound();
            }

            DateTime dateTimeNow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(
                    DateTime.UtcNow, "Europe/Belgrade");

            var firstStartTime = first.DateTimeOfEvent;
            var video= first.Video;

            //check if video is still available
            //all videos are available just one hour
            if (firstStartTime.AddHours(2) >= dateTimeNow)
                return Ok();

            return NoContent();
        }

        [Authorize]
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
                userIds = new List<int>()
            };

            newEvent.userIds.Add(Int32.Parse(idClaim.Value));

            newEvent.Code = GetHashString(newEvent.Name);
            newEventFromBody.Code = newEvent.Code;
            newEventFromBody.CreatorId = newEvent.CreatorId;
            _logger.LogInformation($"New event's code: {newEvent.Code}");


            var collection = _databaseClient.MongoDatabase.GetCollection<Event>("events-collection");
            _logger.LogInformation($"Collection gathered, number of documents inside: {(collection!=null?collection.Find(x=>true).ToList().Count:-1)}");

            if ((await collection.FindAsync(x => x.Code == newEvent.Code)).FirstOrDefault() != null)
                return BadRequest("Existing code!- Event with that code already exists");

            await collection.InsertOneAsync(newEvent);
            _logger.LogInformation($"Inserted new event!");

            var config = new ProducerConfig
            {
                BootstrapServers = "broker:9092",                
                MessageCopyMaxBytes = 15728640,
                ReceiveMessageMaxBytes = 15728640,
                MessageMaxBytes= 15728640,
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

        [Authorize]
        [HttpPost]
        [Route("{code}/UploadFile")]
        public async Task<IActionResult> UploadFile([FromRoute(Name = "code")]string code)
        {
            var collection = _databaseClient.MongoDatabase.GetCollection<Event>("events-collection");
            var eventFromDb = await (await collection.FindAsync(x => x.Code == code)).FirstOrDefaultAsync();
            if (eventFromDb == null)
                return NotFound($"Event with code {code} doesn't exist!");

            _logger.LogInformation("Persisting file");

            try
            {
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files[0];
                _logger.LogInformation($"File found, length: {file.Length}, file name: {file.Name}");
                using (var memoryStream=new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    var bytes=memoryStream.ToArray();

                    //persist to the database
                    _logger.LogInformation($"Persisting file {file.Name} to the database (number of bytes: {bytes.Count()})");

                    var filter = Builders<Event>.Filter.Eq(x => x.Code,code);
                    var update = Builders<Event>.Update.Set(x => x.Video, bytes);
                    collection.UpdateOne(filter, update);

                    return StatusCode(201);
                }
            }
            catch(Exception ex)
            {
                _logger.LogInformation($"Error while persisting video: {ex}");
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("DeleteAll")]
        public async Task<IActionResult> DeleteAll()
        {
            var collection =await _databaseClient.MongoDatabase.GetCollection<Event>("events-collection").DeleteManyAsync(x => true);
            return Ok();
        }

        [Authorize]
        [HttpPost]
        [Route("SubscribeToEvent")]
        public async Task<IActionResult> SubscribeToEvent(string code)
        {
            _logger.LogInformation("Subscribing to event");
            var identity = (ClaimsIdentity)HttpContext.User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            Claim idClaim = Enumerable.ElementAt<Claim>(claims, 2);


            var collection =  _databaseClient.MongoDatabase.GetCollection<Event>("events-collection");
            var document=  (await collection.FindAsync(x => x.Code == code)).FirstOrDefault();
            if (document == null)
                return BadRequest("Event doesn't exist!");

            document.userIds.Add(Int32.Parse(idClaim.Value));
            _logger.LogInformation($"Subscribing to event {code} user with id {idClaim.Value}");

            var update = MongoDB.Driver.Builders<Event>.Update.Set(x => x.userIds, document.userIds);

            collection.UpdateOne(x => x.Code == code, update);

            return Ok();
        }

        [Authorize]
        [HttpPost]
        [Route("UnsubscribeToEvent")]
        public async Task<IActionResult> UnsubscribeToEvent(string code)
        {
            _logger.LogInformation("Unsubscribing from event");
            var identity = (ClaimsIdentity)HttpContext.User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            Claim idClaim = Enumerable.ElementAt<Claim>(claims, 2);
            int id = Int32.Parse(idClaim.Value);

            var collection = _databaseClient.MongoDatabase.GetCollection<Event>("events-collection");
            var document = (await collection.FindAsync(x => x.Code == code)).FirstOrDefault();
            if (document == null)
                return BadRequest("Event doesn't exist!");

            if (!document.userIds.Contains(Int32.Parse(idClaim.Value)))
            {
                return BadRequest("You are not subscribed to this event!");
            }

            _logger.LogInformation($"Unsubscribing to event {code} user with id {idClaim.Value}");

            document.userIds.Remove(id);
            _logger.LogInformation($"User with id {id} has unsubscribed from event with code {code}, left subscribers: {document.userIds.Count}");
            var updater = MongoDB.Driver.Builders<Event>.Update.Set(x => x.userIds, document.userIds);
            collection.UpdateOne(x => x.Code == code, updater);

            return Ok();
        }

        [Authorize]
        [HttpGet]
        [Route("GetMyEvents")]
        public async Task<IActionResult> GetMyEvents()
        {
            _logger.LogInformation("Subscribing to event");
            var identity = (ClaimsIdentity)HttpContext.User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            Claim idClaim = Enumerable.ElementAt<Claim>(claims, 2);
            int id = Int32.Parse(idClaim.Value);

            var collection = _databaseClient.MongoDatabase.GetCollection<Event>("events-collection");
            var list=(await collection.FindAsync(x => x.userIds != null && x.userIds.Contains(id))).ToList();
            DateTime dateTimeNow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(
                DateTime.UtcNow, "Europe/Belgrade");
            list = list.Where(x => x.DateTimeOfEvent.AddHours(2) > dateTimeNow).ToList();

            return Ok(list);
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
