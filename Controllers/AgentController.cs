using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using MossadAgentAPI.Services;
using MossadAgentAPI.Models;
using MossadAgentAPI.Enums;
using IronDomeApi.Utils;
using System.Security.Cryptography.Xml;

namespace MossadAgentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentController : ControllerBase
    {
        private readonly MossadAgentContext _context;
        private readonly ILogger<AgentController> _logger;


        public AgentController(ILogger<AgentController> logger, MossadAgentContext context)
        {

            this._context = context;
            this._logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAgents()
        {
            int status = StatusCodes.Status200OK;
            var agents = await _context.agents.Include(a => a.location)?.ToArrayAsync();
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
            await this._context.SaveChangesAsync();
            status = StatusCodes.Status201Created;
            return StatusCode(
                status,
                HttpUtils.Response(status, new { agent = agent })
                );
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
            agent.location = location;
            this._context.agents.Update(agent);
            await this._context.SaveChangesAsync();

            status = StatusCodes.Status200OK;
            return StatusCode(status, HttpUtils.Response(status, new { agent = agent }));
        }

        [HttpPut("{id}/move")]
        [Produces("application/json")]

        public async Task<IActionResult> MoveAgentLocation(int id,[FromQuery] string direction)
        {
            if (string.IsNullOrEmpty(direction))
            {
                return BadRequest("The direction field is required.");
            }
            if (!DirectionService.DirectionActions.ContainsKey(direction))
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
            DirectionService.DirectionActions[direction](agent.location);
            this._context.agents.Update(agent);
            await this._context.SaveChangesAsync();
            status = StatusCodes.Status200OK;
            return StatusCode(status, HttpUtils.Response(status, new { agent = agent }));
        }

    }
}
