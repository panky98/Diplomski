using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace StreamingMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StreamController : Controller
    {
        public StreamController(IHttpClientFactory httpClientFactory,
                                ILogger<StreamController> logger)
        {
            _httpClientFactory=httpClientFactory;
            _logger = logger;
        }
        
        readonly RedisClient redis = new RedisClient("redis-cache", 6379);
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<StreamController> _logger;  
        
        [HttpGet("{code}")]
        public async Task<IActionResult> Play([FromRoute(Name ="code")] string code)
        {
            _logger.LogInformation($"Starting streaming for event with code {code}");

            //get file and return it
            var bytes = redis.GetBytes(code);
            _logger.LogInformation($"Streaming {bytes.Count()} number of bytes!");
            var stream = new MemoryStream(bytes);

            return File(stream, "video/mp4", enableRangeProcessing:true);
        }

        [HttpGet("{code}/Check")]
        public async Task<IActionResult> Check([FromRoute(Name = "code")] string code)
        {
            //first check is video still available
            _logger.LogInformation($"Checking if event with code {code} is still available");
            if(!redis.ContainsKey($"EXP_{code}"))
            {
                _logger.LogInformation($"Event with code {code} not found");
                return NotFound();
            }

            var expirationDateTimeString = redis.Get<string>($"EXP_{code}");
            var expirationDateTime = DateTime.Parse(expirationDateTimeString);

            DateTime dateTimeNow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Europe/Belgrade");

            if (dateTimeNow>expirationDateTime)
            {
                _logger.LogInformation($"Event with code {code} has expired, cleaning redis cache");
                //clean redis
                if (redis.ContainsKey(code))
                {
                    redis.Remove(code);
                }
                return NoContent();
            }

            _logger.LogInformation($"Event with code {code} is still available, returning 200");
            return Ok();
        }
    }
}
