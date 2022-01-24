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
            var httpClient = _httpClientFactory.CreateClient();

            //first check is video still available
            _logger.LogInformation($"Checking if event with code {code} is still available");
            var response = await httpClient.GetAsync($"http://eventmicroservice/api/Events/{code}/Check");
            _logger.LogInformation($"Response for event with code {code} collected");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogInformation($"Event with code {code} not found");
                return NotFound();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                _logger.LogInformation($"Event with code {code} has expired, cleaning redis cache");
                //clean redis
                if (redis.ContainsKey(code))
                {
                    redis.Remove(code);
                }
                return NoContent();
            }

            _logger.LogInformation($"Event with code {code} still available, starting streaming");
            //get file and return it
            var bytes = redis.GetBytes(code);
            _logger.LogInformation($"Streaming {bytes.Count()} number of bytes!");
            var stream = new MemoryStream(bytes);

            return new FileStreamResult(stream, "video/mp4");
        }
    }
}
