using UnityEngine;

namespace DEMSIFT.Scoring
{
    public static class TrainingSession
    {
        public const int StepCount = 9;

        private const int MaxStepScore = 100;
        private const int MinStepScore = 50;
        private const float SlowestTimeFactor = 3f;

        private static readonly int[] stepScores = new int[StepCount];
        private static readonly float[] stepSeconds = new float[StepCount];
        private static bool leaderboardPending;

        public static int PreparationScore { get; private set; }
        public static int PreparationWrongAttempts { get; private set; }

        public static int Total
        {
            get
            {
                int total = PreparationScore;
                foreach (int score in stepScores) total += score;
                return total;
            }
        }

        public static int StepScoreOf(int step) => stepScores[step - 1];

        public static float StepSecondsOf(int step) => stepSeconds[step - 1];

        // --- Recording ---
        public static void RecordPreparation(int score, int wrongAttemptCount)
        {
            PreparationScore = score;
            PreparationWrongAttempts = wrongAttemptCount;
        }

        public static int RecordStep(int step, float seconds, float parTime)
        {
            int score = StepScoreFor(seconds, parTime);
            stepScores[step - 1] = score;
            stepSeconds[step - 1] = seconds;
            if (step == StepCount) leaderboardPending = true;
            return score;
        }

        public static bool TakeLeaderboardPending()
        {
            bool pending = leaderboardPending;
            leaderboardPending = false;
            return pending;
        }

        // --- Score rule: full score up to the fair time, then down to the minimum at three times it ---
        public static int StepScoreFor(float seconds, float parTime)
        {
            if (parTime <= 0f || seconds <= parTime) return MaxStepScore;

            float slowness = Mathf.Clamp01((seconds - parTime) / (parTime * (SlowestTimeFactor - 1f)));
            return Mathf.RoundToInt(Mathf.Lerp(MaxStepScore, MinStepScore, slowness));
        }

        // --- New player, and play mode without domain reload ---
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Clear()
        {
            System.Array.Clear(stepScores, 0, StepCount);
            System.Array.Clear(stepSeconds, 0, StepCount);
            PreparationScore = 0;
            PreparationWrongAttempts = 0;
            leaderboardPending = false;
        }
    }
}
