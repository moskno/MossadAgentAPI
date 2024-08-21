using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MossadAgentAPI.Services;
using MossadAgentAPI.Models;
using MossadAgentAPI.Enums;

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

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateAgent(Agent agent)
        {
            agent.Status = AgentStatus.Inactive;
            this._context.agents.Add(agent);
            await this._context.SaveChangesAsync();
            return StatusCode(
                StatusCodes.Status201Created,
                new { success = true, agent = agent }
                );
        }
    }
}
