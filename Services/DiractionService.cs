using MossadAgentAPI.Models;
namespace MossadAgentAPI.Services
{
    public static class DirectionService
    {
        public static readonly Dictionary<string, Action<Location>> DirectionActions = new Dictionary<string, Action<Location>>
        {
            { "nw", loc => { loc.y -= 1; loc.x -= 1; } },
            { "n", loc => loc.y -= 1 },
            { "ne", loc => { loc.y -= 1; loc.x += 1; } },
            { "w", loc => loc.x -= 1 },
            { "e", loc => loc.x += 1 },
            { "sw", loc => { loc.y += 1; loc.x -= 1; } },
            { "s", loc => loc.y += 1 },
            { "se", loc => { loc.y += 1; loc.x += 1; } }
        };
    }
}