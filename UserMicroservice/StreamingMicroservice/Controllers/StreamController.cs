using Microsoft.AspNetCore.Mvc;
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
        public StreamController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory=httpClientFactory;
        }
        
        readonly RedisClient redis = new RedisClient("redis-cache", 6380);
        private readonly IHttpClientFactory _httpClientFactory;
        
        [HttpGet("{code}")]
        public async Task<IActionResult> Play([FromRoute(Name ="code")] string code)
        {
            var httpClient = _httpClientFactory.CreateClient();

            //first check is video still available
            var response = await httpClient.GetAsync($"http://api/eventmicroservice/Events/{code}/Check");

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return NotFound();
            else if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                //clean redis
                if (redis.ContainsKey(code))
                {
                    redis.Remove(code);
                }
                return NoContent();
            }

            //get file and return it
            var bytes = redis.GetBytes(code);
            var stream = new MemoryStream(bytes);

            return File(stream,"video/mp4");
        }
    }
}
