using Microsoft.EntityFrameworkCore;
using MossadAgentAPI.Enums;
using MossadAgentAPI.Models;
using MossadAgentAPI.Services;
using System.Reflection;

namespace MossadAgentAPI.Services
{
    public class MissionService
    {
        private readonly MossadAgentContext _context;
        private readonly DistanceCalculate _distanceCheck;

        public MissionService(MossadAgentContext context, DistanceCalculate distanceCheck)
        {
            this._context = context;
            _distanceCheck = distanceCheck;
        }

        public async Task CalculateMissionAAsync(Agent agent)
        {
            var targets = await _context.targets.Include(ag => ag.location).ToListAsync();
            if (targets.Any()) 
            {
                foreach (var target in targets)
                {
                    var distance = _distanceCheck.CalculateDistance(agent.location, target.location);
                    if (distance < +200)
                    {
                        await CreateMissionAsync(agent, target);
                    }
                }
            }
        }
        public async Task CalculateMissionTAsync(Target target)
        {
            var agents = await this._context.agents.Include(ta => ta.location).ToListAsync();
            if (agents.Any())
            {
                foreach (var agent in agents)
                {
                    var distance = _distanceCheck.CalculateDistance(agent.location, target.location);
                    if (distance < +200)
                    {
                        await CreateMissionAsync(agent, target);
                    }
                }
            }
        }

        private async Task CreateMissionAsync(Agent agent, Target target)
        {
            try
            {
                var mission = new Mission
                {
                    AgentId = agent.Id,
                    TargetId = target.Id,
                    TimeLeft = double.MaxValue,
                    ExecutionTime = TimeOnly.MinValue,
                    Status = MissionStatus.Ready
                };
                this._context.missions.Add(mission);
                await this._context.SaveChangesAsync();

            }
            catch (Exception ex) { }

            
        }
        
        public string MovingDirection(Location agentLoc, Location targetLoc)
        {
            int dirX = targetLoc.x - agentLoc.x;
            int dirY = targetLoc.y - agentLoc.y;

            string dir = "";

            if (dirY < 0) { dir += "n"; }
            if (dirY > 0) { dir += "s"; }
            if (dirX > 0) { dir += "e"; }
            if (dirX < 0) { dir += "w"; }
                
            return dir;
        }
    }
}

        //private async Task CalculateMissionsAsync<T>(IEnumerable<T> entities, Func<T, Task> checkAndCreateMission)
        //{
        //    foreach (var entity in entities)
        //    {
        //        await checkAndCreateMission(entity);
        //    }
        //}

        //public async Task CalculateMissionAAsync(Agent agent)
        //{
        //    var targets = await _context.targets.Include(ag => ag.location).ToListAsync();
        //    await CalculateMissionsAsync(targets, async target =>
        //    {
        //        await CheckMissionAsync(agent, target);
        //    });
        //}
        //public async Task CalculateMissionTAsync(Target target)
        //{
        //    var agents = await this._context.agents.Include(ta => ta.location).ToListAsync();
        //    await CalculateMissionsAsync(agents, async agent =>
        //    {
        //        await CheckMissionAsync(agent, target);
        //    });
        //}

        //private async Task CheckMissionAsync(Agent agent, Target target)
        //{
        //    var distance = _distanceCheck.CalculateDistance(agent.location, target.location);
        //    if (distance < 200)
        //    {
        //        bool missionExists = await _context.missions.AnyAsync(m => m.AgentId == agent.Id && m.TargetId == target.Id);

        //        if (!missionExists)
        //        {
        //            await CreateMissionAsync(agent, target);
        //        }
        //    }
        //}