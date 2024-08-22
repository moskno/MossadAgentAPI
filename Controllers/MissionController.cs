using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using MossadAgentAPI.Services;
using MossadAgentAPI.Models;
using MossadAgentAPI.Enums;
using IronDomeApi.Utils;
using System.Security.Cryptography.Xml;
using System.Reflection;

namespace MossadAgentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MissionController : ControllerBase
    {
        private readonly MossadAgentContext _context;
        private readonly ILogger<MissionController> _logger;


        public MissionController(ILogger<MissionController> logger, MossadAgentContext context)
        {

            this._context = context;
            this._logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetMissions()
        {
            int status = StatusCodes.Status200OK;
            var missions = await _context.missions.ToArrayAsync();
            return StatusCode(
            status,
                HttpUtils.Response(status, new { missions = missions })
                );
        }


        [HttpPost("/missions/update")]
        public async Task<IActionResult> UpdateTimeLeft()
        {
            ........
        }


        [HttpPut("{id}")]
        [Produces("application/json")]
        public async Task<IActionResult> ActiveStatus(int id)
        {
            int status;
            Mission mission = this._context.missions.FirstOrDefault(mi => mi.Id == id);
            if (mission == null)
            {
                status = StatusCodes.Status404NotFound;
                return StatusCode(status, HttpUtils.Response(status, "mission not found"));
            }
            mission.Status = MissionStatus.Active;
            this._context.missions.Update(mission);
            await this._context.SaveChangesAsync();
            status = StatusCodes.Status200OK;
            return StatusCode(status, HttpUtils.Response(status, new { mission = mission }));
        }

    }
}
