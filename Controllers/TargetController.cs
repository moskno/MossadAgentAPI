using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using MossadAgentAPI.Services;
using MossadAgentAPI.Models;
using MossadAgentAPI.Enums;
using MossadAgentAPI.Utils;
using System.Security.Cryptography.Xml;

namespace MossadAgentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TargetController : ControllerBase
    {
        private readonly MossadAgentContext _context;
        private readonly ILogger<TargetController> _logger;
        private readonly MissionService _missionService;


        public TargetController(ILogger<TargetController> logger, MossadAgentContext context, MissionService missionService)
        {

            this._context = context;
            this._logger = logger;
            _missionService = missionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTargets()
        {
            int status = StatusCodes.Status200OK;
            var targets = await _context.targets.Include(t => t.location).ToArrayAsync();
            return StatusCode(
                status,
                HttpUtils.Response(status, new { targets = targets })
                );
        }

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateTarget(Target target)
        {
            int status;
            if (target.location == null)
            {
                target.location = new Location();
            }
            target.Status = TargetStatus.Live;
            this._context.targets.Add(target);
            await Task.Run(async () =>
            {
                await this._missionService.CalculateMissionTAsync(target);
            });
            await this._context.SaveChangesAsync();
            status = StatusCodes.Status201Created;
            return StatusCode(
                status,
                HttpUtils.Response(status, new { target = target })
                );
        }

        [HttpPut("{id}/pin")]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateTargetLocation(int id, Location location)
        {
            //if (Location.IsNullOrEmpty(location.ToString()))
            //{
            //    return BadRequest("The direction field is required.");
            //}
            int status;
            Target target = this._context.targets.FirstOrDefault(tar => tar.Id == id);
            if (target == null)
            {
                status = StatusCodes.Status404NotFound;
                return StatusCode(status, HttpUtils.Response(status, "status not found"));
            }
            if (target.location == null)
            {
                target.location = new Location();
            }
            target.location = location;
            this._context.targets.Update(target);
            await this._context.SaveChangesAsync();
            await Task.Run(async () => { await this._missionService.CalculateMissionTAsync(target); });
            status = StatusCodes.Status200OK;
            return StatusCode(status, HttpUtils.Response(status, new { target = target }));
        }

        [HttpPut("{id}/move")]
        [Produces("application/json")]

        public async Task<IActionResult> MoveTargetLocation(int id,[FromBody] Direction direction)
        {
            string direct = direction.direction;
            if (string.IsNullOrEmpty(direct))
            {
                return BadRequest("The direction field is required.");
            }
            if (!DirectionService.DirectionActions.ContainsKey(direct))
            {
                return BadRequest("Invalid direction specified.");
            }
            int status;
            Target target = await this._context.targets.Include(tar => tar.location).FirstOrDefaultAsync(tar => tar.Id == id);
            if (target == null)
            {
                status = StatusCodes.Status404NotFound;
                return StatusCode(status, HttpUtils.Response(status, "target not found"));
            }
            if (target.location == null)
            {
                target.location = new Location();
            }
            DirectionService.DirectionActions[direct](target.location);
            if (!TryValidateModel(target.location))
            {
                return BadRequest(HttpUtils.Response(StatusCodes.Status400BadRequest,
                    new { message = "Movement would result in going out of bounds.", currentLocation = target.location }));
            }
            this._context.targets.Update(target);
            await this._context.SaveChangesAsync();
            await Task.Run(async () => { await this._missionService.CalculateMissionTAsync(target); });
            status = StatusCodes.Status200OK;
            return StatusCode(status, HttpUtils.Response(status, new { target = target }));
        }

    }
}
