using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace DockerRedisStudy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IConnectionMultiplexer _redisConnection;

        public BookController(IConnectionMultiplexer redisConnection)
        {
            _redisConnection = redisConnection;
        }

        [HttpPost("set")]
        public async Task<IActionResult> SetData([FromQuery] string key, [FromQuery] string value)
        {
            var db = _redisConnection.GetDatabase();
            await db.StringSetAsync(key, value);
            return Ok(new { Key = key, Value = value, Message = "Data saved successfully in Redis." });
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetData([FromQuery] string key)
        {
            var db = _redisConnection.GetDatabase();
            var value = await db.StringGetAsync(key);
            if (value.IsNullOrEmpty)
            {
                return NotFound("Data not found.");
            }

            return Ok(new { Key = key, Value = value.ToString() });
        }

        [HttpGet("test-redis-connection")]
        public IActionResult TestRedisConnection()
        {
            try
            {
                var db = _redisConnection.GetDatabase();
                var result = db.Ping();
                return Ok(new { Message = "Connected to Redis", Ping = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to connect to Redis", Error = ex.Message });
            }
        }
    }
}
