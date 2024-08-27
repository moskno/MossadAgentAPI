using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using MossadAgentAPI.Services;
using MossadAgentAPI.Models;
using MossadAgentAPI.Enums;
using MossadAgentAPI.Utils;

namespace MossadAgentAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AgentsController : ControllerBase
    {
        private readonly MossadAgentContext _context;
        private readonly ILogger<AgentsController> _logger;
        private readonly MissionService _missionService;


        public AgentsController(ILogger<AgentsController> logger, MossadAgentContext context, MissionService missionService)
        {

            this._context = context;
            this._logger = logger;
            this._missionService = missionService;   
        }

        [HttpGet]
        public async Task<IActionResult> GetAgents()
        {
            int status = StatusCodes.Status200OK;
            var agents = await this._context.agents.Include(a => a.location).ToArrayAsync();
            return StatusCode(
                status,
                HttpUtils.Response(status, new { agents = agents })
                );
        }

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateAgent(Agent agent)
        {
            int status;
            agent.Status = AgentStatus.Inactive;
            this._context.agents.Add(agent);
            //await Task.Run(async () =>
            //{
            //    await this._missionService.CalculateMissionAAsync(agent);
            //});
            await this._context.SaveChangesAsync();
            status = StatusCodes.Status201Created;
            return StatusCode(status, new { id = agent.Id });
                //status,
                //HttpUtils.Response(status, new { id = agent.Id })
                //);
        }

        [HttpPut("{id}/pin")]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateAgentLocation(int id, Location location)
        {
            int status;

            Agent agent = this._context.agents.FirstOrDefault(ag => ag.Id == id);
            if (agent == null)
            {
                status = StatusCodes.Status404NotFound;
                return StatusCode(status, HttpUtils.Response(status, "agent not found"));
            }
            if (agent.location == null)
            {
                agent.location = new Location();
            }   
            agent.location = location;
            this._context.agents.Update(agent);
            await this._context.SaveChangesAsync();
            await Task.Run(async () =>
            {
                await this._missionService.CalculateMissionAAsync(agent);
            });
            status = StatusCodes.Status200OK;
            return StatusCode(status, HttpUtils.Response(status, new { agent = agent }));
        }

        [HttpPut("{id}/move")]
        [Produces("application/json")]

        public async Task<IActionResult> MoveAgentLocation(int id,[FromBody] Direction direction)
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
            Agent agent = await this._context.agents.Include(ag => ag.location).FirstOrDefaultAsync(ag => ag.Id == id);
            if (agent == null)
            {
                status = StatusCodes.Status404NotFound;
                return StatusCode(status, HttpUtils.Response(status, "agent not found"));
            }
            if (agent.location == null)
            {
                agent.location = new Location();
            }
            DirectionService.DirectionActions[direct](agent.location);
            if (!TryValidateModel(agent.location))
            {
                return BadRequest(HttpUtils.Response(StatusCodes.Status400BadRequest,
                    new { message = "Movement would result in going out of bounds.", currentLocation = agent.location }));
            }
            this._context.agents.Update(agent);
            await Task.Run(async () =>
            {
                await this._missionService.CalculateMissionAAsync(agent);
            });
            await this._context.SaveChangesAsync();
            status = StatusCodes.Status200OK;
            return StatusCode(status, HttpUtils.Response(status, new { agent = agent }));
        }

    }
}
