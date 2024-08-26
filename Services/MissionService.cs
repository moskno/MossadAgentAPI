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

        public void CalculateMissionA(Agent agent)
        {
            var targets = _context.targets.Include(ag => ag.location).ToList();
            foreach (var target in targets) 
            {
                CheckMission(agent, target);
            }
        }
        public void CalculateMissionT(Target target)
        {
            var agents = this._context.agents.Include(ta => ta.location).ToList();
            foreach (var agent in agents)
            {
                CheckMission(agent, target);
            }
        }

        private void CheckMission(Agent agent, Target target)
        {
            var distance = _distanceCheck.CalculateDistance(agent.location, target.location);
            if (distance< +200)
            {
                bool missionExists = _context.missions.Any(m => m.AgentId == agent.Id && m.TargetId == target.Id);

                if (!missionExists)
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
