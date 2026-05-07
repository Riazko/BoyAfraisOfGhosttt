using System.Collections.Generic;
using System.Linq;
using BoyAfraidOfGhosts.Models;
using BoyAfraidOfGhosts.Helpers;

namespace BoyAfraidOfGhosts.Controllers
{
    public class GreedyFlashSelector
    {
        public List<GhostModel> SelectGhostsToKill(List<GhostModel> ghosts, Vector2D playerPosition,
            Vector2D flashDirection, double halfAngleDegrees)
        {
            var toKill = ghosts
                .Where(p => p.IsAlive)
                .Where(p => GeometryHelper.IsPointInCone(playerPosition,
                flashDirection, halfAngleDegrees, p.Position, Settings.FlashRadius))
                .OrderBy(p => p.Position.DistanceTo(playerPosition))
                .Take(Settings.MaxFlashKills)
                .ToList();
            return toKill;
        }
    }
}