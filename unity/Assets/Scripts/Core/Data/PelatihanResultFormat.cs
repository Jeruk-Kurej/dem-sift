using System.Collections.Generic;
using System.Globalization;
using DEMSIFT.Scoring;

namespace DEMSIFT.Data
{
    public class PelatihanResultFormat : ResultFormat
    {
        public override IEnumerable<string> ScoreHeaders()
        {
            yield return "Skor Persiapan";
            yield return "Jumlah Salah Persiapan";
            for (int step = 1; step <= TrainingSession.StepCount; step++) yield return $"Skor Langkah {step}";
            for (int step = 1; step <= TrainingSession.StepCount; step++) yield return $"Detik Langkah {step}";
        }

        public override IEnumerable<string> CurrentScores()
        {
            yield return TrainingSession.PreparationScore.ToString();
            yield return TrainingSession.PreparationWrongAttempts.ToString();
            for (int step = 1; step <= TrainingSession.StepCount; step++) yield return TrainingSession.StepScoreOf(step).ToString();
            for (int step = 1; step <= TrainingSession.StepCount; step++)
            {
                yield return TrainingSession.StepSecondsOf(step).ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        public override int CurrentTotal => TrainingSession.Total;
    }
}
