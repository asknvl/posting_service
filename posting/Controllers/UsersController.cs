using Microsoft.AspNetCore.Mvc;
using posting.Dtos.LoaderBot;
using posting.Models.Users;
using posting.Services.MongoDBService;

namespace posting.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {        
        readonly IMongoDBService mongoDBService;
        readonly ILogger logger;       

        public UsersController(IMongoDBService mongoDBService, ILogger<UsersController> logger)
        {
            this.mongoDBService = mongoDBService;
            this.logger = logger;
        }

        /// <summary>
        /// Returnes list of verified bot users by direction_id
        /// </summary>
        /// <param name="direction_id"></param>
        /// <returns></returns>
        [HttpGet("GetUsers")]
        [ProducesResponseType(typeof(List<UserModel>), StatusCodes.Status200OK, "application/json")]
        public async Task<IActionResult> GetUsers([FromQuery] int direction_id)
        {
            logger.LogInformation($"Getting users direction={direction_id}");

            List<UserModel> users = new List<UserModel>();

            try
            {
                users = await mongoDBService.GetUsers(direction_id);
                if (users.Count == 0)
                    return NotFound(users);

            } catch (Exception ex)
            {
                logger.LogError($"GetUsers direction_id={direction_id}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return Ok(users);
        }

        /// <summary>
        /// Deletes user by id, returns updated user list by direction_id
        /// </summary>
        /// <param name="direction_id"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("RemoveUser")]
        [ProducesResponseType(typeof(List<UserModel>), StatusCodes.Status200OK, "application/json")]
        public async Task<IActionResult> RemoveUser([FromQuery] int direction_id, [FromQuery] string id)
        {
            logger.LogInformation($"RemoveUser id={id}");

            try
            {
                var result = await mongoDBService.RemoveUser(direction_id, id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError($"RemoveUser id={id}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
