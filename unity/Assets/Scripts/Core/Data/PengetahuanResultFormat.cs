using System.Collections.Generic;
using DEMSIFT.Scoring;

namespace DEMSIFT.Data
{
    public class PengetahuanResultFormat : ResultFormat
    {
        public override IEnumerable<string> ScoreHeaders()
        {
            for (int soal = 1; soal <= ScoreSession.SoalCount; soal++) yield return $"Skor Soal {soal}";
            for (int soal = 1; soal <= ScoreSession.SoalCount; soal++) yield return $"Jumlah Salah Soal {soal}";
        }

        public override IEnumerable<string> CurrentScores()
        {
            for (int soal = 1; soal <= ScoreSession.SoalCount; soal++) yield return ScoreSession.ScoreOf(soal).ToString();
            for (int soal = 1; soal <= ScoreSession.SoalCount; soal++) yield return ScoreSession.WrongAttemptsOf(soal).ToString();
        }

        public override int CurrentTotal => ScoreSession.Total;
    }
}
