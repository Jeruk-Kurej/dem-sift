using UnityEngine;

namespace DEMSIFT.Scoring
{
    public static class ScoreSession
    {
        public const int SoalCount = 6;

        private static readonly int[] scores = new int[SoalCount];
        private static readonly int[] wrongAttempts = new int[SoalCount];
        private static bool leaderboardPending;

        public static int Total
        {
            get
            {
                int total = 0;
                foreach (int score in scores) total += score;
                return total;
            }
        }

        public static bool IsComplete
        {
            get
            {
                foreach (int score in scores)
                {
                    if (score == 0) return false;
                }
                return true;
            }
        }

        public static int ScoreOf(int soalNumber) => scores[soalNumber - 1];

        public static int WrongAttemptsOf(int soalNumber) => wrongAttempts[soalNumber - 1];

        public static void Record(int soalNumber, int score, int wrongAttemptCount)
        {
            scores[soalNumber - 1] = score;
            wrongAttempts[soalNumber - 1] = wrongAttemptCount;
            if (IsComplete) leaderboardPending = true;
        }

        public static bool TakeLeaderboardPending()
        {
            bool pending = leaderboardPending;
            leaderboardPending = false;
            return pending;
        }

        // --- New player, and play mode without domain reload ---
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Clear()
        {
            System.Array.Clear(scores, 0, SoalCount);
            System.Array.Clear(wrongAttempts, 0, SoalCount);
            leaderboardPending = false;
        }
    }
}
