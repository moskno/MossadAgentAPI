using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using MossadAgentAPI.Controllers;
using MossadAgentAPI.Services;
using MossadAgentAPI.Models;
using MossadAgentAPI.Enums;
using MossadAgentAPI.Utils;
using System.Security.Cryptography.Xml;
using System.Reflection;
using Microsoft.IdentityModel.Tokens;

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

        //[HttpGet]
        //public async Task<IActionResult> GetMissionDetails()
        //{
        //    int status = StatusCodes.Status200OK;
        //    var missions = await _context.missions.ToListAsync();
        //    var missionDetails = new List<object>();
        //    foreach (var mission in missions)
        //    {
        //        var agent = await _context.agents
        //            .Include(a => a.location)
        //            .FirstOrDefaultAsync(a => a.Id == mission.AgentId);

        //        var target = await _context.targets
        //            .Include(t => t.location)
        //            .FirstOrDefaultAsync(t => t.Id == mission.TargetId);

        //        if (agent == null || target == null) continue;
        //        var distance = _distanceCalculate.CalculateDistance(agent.location, target.location);
        //        missionDetails.Add(new
        //        {
        //            MissionId = mission.Id,
        //            AgentId = mission.AgentId,
        //            AgentNickname = agent.Nickname,
        //            AgentLocation = agent.location,
        //            TargetId = mission.TargetId,
        //            TargetName = target.Name,
        //            TargetRole = target.Role,
        //            TargetLocation = target.location,
        //            Distance = distance,
        //            TimeLeft = mission.TimeLeft,
        //            ExecutionTime = mission.ExecutionTime,
        //            Status = mission.Status
        //        });
        //    }
        //    return StatusCode(
        //        status,
        //        HttpUtils.Response(status, new { missionDetails = missionDetails })
        //    );
        //}


        [HttpPut("{id}")]
        [Produces("application/json")]
        public async Task<IActionResult> ActiveStatus(int id)
            {
            int status;
            Mission mission = await this._context.missions.FirstOrDefaultAsync(mi => mi.Id == id);
            if (mission == null)
            {
                status = StatusCodes.Status404NotFound;
                return StatusCode(status, HttpUtils.Response(status, "mission not found"));
            }
            var agent = await _context.agents.FirstOrDefaultAsync(a => a.Id == mission.AgentId);
            agent.Status = AgentStatus.Active;
            mission.Status = MissionStatus.Active;
            this._context.missions.Update(mission);
            this._context.agents.Update(agent);
            await this._context.SaveChangesAsync();
            status = StatusCodes.Status200OK;
            return StatusCode(status, HttpUtils.Response(status, new { mission = mission }));
        }


        [HttpPost("update")]
        public async Task<IActionResult> UpdateToKill()
        {
            int status = StatusCodes.Status200OK;
            var missions = await _context.missions.ToArrayAsync();
            foreach ( Mission mission in missions)
            {
                if (mission.Status == MissionStatus.Active)
                {
                    var agent = await _context.agents.Include(a => a.location).FirstOrDefaultAsync(a => a.Id == mission.AgentId);
                    var target = await _context.targets.Include(t => t.location).FirstOrDefaultAsync(t => t.Id == mission.TargetId);
                    if (agent == null || target == null)
                    {
                        return StatusCode(StatusCodes.Status404NotFound, "Agent or Target not found for mission");
                    }
                    mission.TimeLeft = this._distanceCalculate.CalculateDistance(agent.location, target.location);
                    string MoveDirection = this._missionService.MovingDirection(agent.location, target.location);
                    if (MoveDirection.IsNullOrEmpty())
                    {
                        agent.Status = AgentStatus.Inactive;
                        target.Status = TargetStatus.Die;
                        target.location = null;
                        mission.Status = MissionStatus.Completed;
                        this._context.agents.Update(agent);
                        this._context.targets.Update(target);
                        this._context.missions.Update(mission);
                    }
                    else
                    {
                        DirectionService.DirectionActions[MoveDirection](agent.location);
                        this._context.agents.Update(agent);
                    }
                    await this._context.SaveChangesAsync();
                }
            }
            return StatusCode(
                status,
                HttpUtils.Response(status, new { missions = missions })
                );
        }

    }
}
