using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using OTest.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataController : ControllerBase
    {
        private readonly MongoDbService _mongoDbService;
        private readonly ILogger<DataController> _logger;

        public DataController(MongoDbService mongoDbService, ILogger<DataController> logger)
        {
            _mongoDbService = mongoDbService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BsonDocument>>> Get()
        {
            try
            {
                var users = await _mongoDbService.GetUsers();
                _logger.LogInformation("Retrieved users from MongoDB.");
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving users from MongoDB.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] BsonDocument userDocument)
        //מבצעת ולידציה אוטומטית על הנתונים באמצעות המודל המחובר ל-Data Annotation.
         {  await _mongoDbService.AddUser(userDocument);
            return CreatedAtAction(nameof(Get), new { id = userDocument["_id"].ToString() }, userDocument);
        }
    }
}