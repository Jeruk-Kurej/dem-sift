using System;
using UnityEngine;

namespace DEMSIFT.Scoring
{
    public class SoalScore : MonoBehaviour
    {
        private const int StartScore = 100;
        private const int PenaltyPerWrongAttempt = 10;
        private const int MinScore = 10;

        [SerializeField] private int soalNumber = 1;
        [SerializeField] private ResultPopup resultPopup;

        private int wrongAttempts;

        public event Action<int> Changed;

        public int Current => Mathf.Max(MinScore, StartScore - wrongAttempts * PenaltyPerWrongAttempt);

        // --- Called by the quiz and puzzle controllers ---
        public int RegisterWrongAttempt()
        {
            int before = Current;
            wrongAttempts++;

            int lostPoints = before - Current;
            if (lostPoints > 0) Changed?.Invoke(Current);
            return lostPoints;
        }

        public void Complete()
        {
            ScoreSession.Record(soalNumber, Current, wrongAttempts);
            resultPopup.ShowCorrect(Current);
        }
    }
}
