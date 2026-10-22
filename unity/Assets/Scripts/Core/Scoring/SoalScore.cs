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
        private bool isCompleted;

        public event Action<int> Changed;
        public event Action Completed;

        public int Current => Mathf.Max(MinScore, StartScore - wrongAttempts * PenaltyPerWrongAttempt);

        // --- Called by the quiz and puzzle controllers ---
        public int RegisterWrongAttempt()
        {
            if (isCompleted) return 0;

            int before = Current;
            wrongAttempts++;

            int lostPoints = before - Current;
            if (lostPoints > 0) Changed?.Invoke(Current);
            return lostPoints;
        }

        public void Complete()
        {
            if (isCompleted) return;

            isCompleted = true;
            ScoreSession.Record(soalNumber, Current, wrongAttempts);
            resultPopup.ShowCorrect(Current);
            Completed?.Invoke();
        }
    }
}
