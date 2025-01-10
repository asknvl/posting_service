using Microsoft.AspNetCore.Mvc;
using posting.Dtos.LoaderBot;
using posting.Models.LoaderBot;
using posting.Services.LoaderBotService;
using Swashbuckle.AspNetCore.Annotations;

namespace posting.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoaderBotServiceController : ControllerBase
    {
        readonly ILoaderBotService loaderBotService;
        readonly ILogger logger;

        public LoaderBotServiceController(ILoaderBotService loaderBotService, ILogger<LoaderBotServiceController> logger) {
            this.loaderBotService = loaderBotService;
            this.logger = logger;
        }

        /// <summary>        
        /// Returns loader bot instance by direction_id
        /// </summary>
        /// <param name="direction_id"></param>
        /// <returns>Loader bot dto if found</returns>
        [ProducesResponseType(typeof(LoaderBotDto), StatusCodes.Status200OK, "application/json")]
        [HttpGet("GetLoaderBot")]        
        public async Task<IActionResult> GetLoaderBot([FromQuery] int direction_id)
        {
            logger.LogInformation($"Getting LoaderBot direction={direction_id}");
            LoaderBotDto loaderBotDto = null;
            try
            {
                loaderBotDto = loaderBotService.GetLoaderBot(direction_id);

            } catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            } catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message); 
            }
            return Ok(loaderBotDto);
        }

        /// <summary>
        /// Toggles loader bot state
        /// </summary>
        /// <param name="loaderBotDto">Essential: direction_id, is_running</param>
        /// <returns>Updated loader bot dto</returns>        
        [HttpPost("ToggleLoaderBot")]
        [ProducesResponseType(typeof(LoaderBotDto), StatusCodes.Status200OK, "application/json")]        
        public async Task<IActionResult> ToggleLoaderBot([FromBody] LoaderBotDto loaderBotDto )        {
            try
            {
                await loaderBotService.ToggleLoaderBot(loaderBotDto.direction_id, loaderBotDto.is_active);
                var bot = loaderBotService.GetLoaderBot(loaderBotDto.direction_id);
                return Ok(bot);

            } catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }            
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }            
        }
    }
}
