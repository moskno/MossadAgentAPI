using Microsoft.EntityFrameworkCore;
using MossadAgentAPI.Enums;
using MossadAgentAPI.Models;
using MossadAgentAPI.Services;

namespace MossadAgentAPI.Services
{
    public class MissionService
    {
        private readonly MossadAgentContext _context;
        private readonly DistanceCheck _distanceCheck;

        public MissionService(MossadAgentContext context, DistanceCheck distanceCheck)
        {
            this._context = context;
            _distanceCheck = distanceCheck;
        }

        public void CalculateMission(Agent agent)
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

        public void CalculateMission(Target target)
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
                TimeLeft = TimeOnly.MaxValue,
                ExecutionTime = TimeOnly.MinValue,
                Status = MissionStatus.Ready
            };
            this._context.missions.Add(mission);
            this._context.SaveChangesAsync();

        }

        //public void CleanupMissions(Agent agent)
        //{
        //    var missions = _context.missions.Include(m => m.Target).Where(m => m.AgentId == agent.Id && m.Status == MissionStatus.Active)
        //                                     .ToList();
        //    foreach (var mission in missions)
        //    {
        //        var distance = CalculateDistance(agent.Location, mission.Target.Location);
        //        if (distance > 200)
        //        {
        //            _context.Missions.Remove(mission);
        //        }
        //    }
        //    _context.SaveChanges();
        //}

    }
}
