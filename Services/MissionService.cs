using Microsoft.EntityFrameworkCore;
using MossadAgentAPI.Enums;
using MossadAgentAPI.Models;
using MossadAgentAPI.Services;

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

        public void CalculateMissionA(Agent agent)
        {
            var targets = _context.targets.ToList();
            foreach (var target in targets) 
            {
                var distance = _distanceCheck.CalculateDistance(agent.location, target.location);
                if (distance < +200)
                {
                    CreateMission(agent, target);
                }
            }
        }

        public void CalculateMissionT(Target target)
        {
            var agents = _context.agents.ToList();
            foreach (var agent in agents)
            {
                var distance = _distanceCheck.CalculateDistance(agent.location, target.location);
                if (distance < +200)
                {
                    CreateMission(agent, target);
                }
            }
        }

        private void CreateMission(Agent agent, Target target)
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
            this._context.SaveChangesAsync();

        }



    }
}
