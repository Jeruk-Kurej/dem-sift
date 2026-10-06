using DEMSIFT.Scoring;
using UnityEngine;

namespace DEMSIFT.Leaderboard
{
    public class LeaderboardAutoOpen : MonoBehaviour
    {
        [SerializeField] private LeaderboardPanel panel;

        private void Start()
        {
            if (ScoreSession.IsComplete) panel.OpenOnCurrentPlayer();
        }
    }
}
