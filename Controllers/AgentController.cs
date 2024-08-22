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

        //private static readonly Dictionary<string, Action<Location>> DirectionActions = new Dictionary<string, Action<Location>>
        //    {
        //        { "nw", loc => { loc.y -= 1; loc.x -= 1; } },
        //        { "n", loc => loc.y -= 1 },
        //        { "ne", loc => { loc.y -= 1; loc.x += 1; } },
        //        { "w", loc => loc.x -= 1 },
        //        { "e", loc => loc.x += 1 },
        //        { "sw", loc => { loc.y += 1; loc.x -= 1; } },
        //        { "s", loc => loc.y += 1 },
        //        { "se", loc => { loc.y += 1; loc.x += 1; } }
        //    };

        [HttpPut("{id}/move")]
        [Produces("application/json")]

        public async Task<IActionResult> MoveAgentLocation(int id,[FromQuery] string direction)
        {
            if (string.IsNullOrEmpty(direction))
            {
                return BadRequest("The direction field is required.");
            }
            int status;
            Agent agent = await this._context.agents.Include(ag => ag.location).FirstOrDefaultAsync(ag => ag.Id == id);
            if (agent == null)
            {
                status = StatusCodes.Status404NotFound;
                return StatusCode(status, HttpUtils.Response(status, "agent not found"));
            }
            switch (direction)
            {
                case "nw":
                    direction = "nw";
                    agent.location.y -= 1;
                    agent.location.x -= 1;
                    break;
                case "n":
                    direction = "n";
                    agent.location.y -= 1;
                        break;
                case "ne":
                    direction = "ne";
                    agent.location.y -= 1;
                    agent.location.x += 1;
                    break;
                case "w":
                    direction = "w";
                    agent.location.x -= 1;
                    break;
                case "e":
                    direction = "e";
                    agent.location.x += 1;
                    break;
                case "sw":
                    direction = "sw";
                    agent.location.y += 1;
                    agent.location.x -= 1;
                    break;
                case "s":
                    direction = "s";
                    agent.location.y += 1;
                    break;
                case "se":
                    direction = "se";
                    agent.location.y += 1;
                    agent.location.x += 1;
                    break;
                default:
                    return BadRequest("Invalid direction specified.");
            }
            this._context.agents.Update(agent);
            await this._context.SaveChangesAsync();
            status = StatusCodes.Status200OK;
            return StatusCode(status, HttpUtils.Response(status, new { agent = agent }));
        }

    }
}
