using System.Collections.Generic;
using System.Linq;
using DEMSIFT.Data;

namespace DEMSIFT.Leaderboard
{
    public static class LeaderboardRanking
    {
        public static List<PlayerResult> Build(IEnumerable<PlayerResult> results)
        {
            var bestByPlayer = new Dictionary<string, PlayerResult>();
            var firstSeenOrder = new List<string>();

            foreach (PlayerResult result in results)
            {
                if (!bestByPlayer.TryGetValue(result.Key, out PlayerResult best))
                {
                    firstSeenOrder.Add(result.Key);
                    bestByPlayer[result.Key] = result;
                }
                else if (result.Total > best.Total)
                {
                    bestByPlayer[result.Key] = result;
                }
            }

            return firstSeenOrder
                .Select(key => bestByPlayer[key])
                .OrderByDescending(result => result.Total)
                .ToList();
        }
    }
}
