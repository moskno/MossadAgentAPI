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
        private readonly MissionService _missionService;
        private readonly DistanceCalculate _distanceCalculate;

        public MissionController(ILogger<MissionController> logger, MossadAgentContext context, MissionService missionService, DistanceCalculate distanceCalculate)
        {

            this._context = context;
            this._logger = logger;
            this._missionService = missionService;
            this._distanceCalculate = distanceCalculate;
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


        [HttpPost("/missions/update")]
        public async Task<IActionResult> UpdateTimeLeft()
        {
            int status = StatusCodes.Status200OK;
            var missions = await _context.missions.ToArrayAsync();
            foreach ( Mission mission in missions)
            {
                var agent = await _context.agents.Include(a => a.location).FirstOrDefaultAsync(a => a.Id == mission.AgentId);
                var target = await _context.targets.Include(t => t.location).FirstOrDefaultAsync(t => t.Id == mission.AgentId);
                if (agent == null || target == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, "Agent or Target not found for mission");
                }
                mission.TimeLeft = this._distanceCalculate.CalculateDistance(agent.location, target.location);
                await this._context.SaveChangesAsync();
            }
            return StatusCode(
                status,
                HttpUtils.Response(status, new { missions = missions })
                );
        }

    }
}
