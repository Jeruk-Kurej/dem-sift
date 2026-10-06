using DEMSIFT.Player;
using DEMSIFT.Scoring;
using UnityEngine;

namespace DEMSIFT.Data
{
    public class ResultSaver : MonoBehaviour
    {
        [SerializeField] private SoalScore lastSoal;

        private void OnEnable()
        {
            lastSoal.Completed += Save;
        }

        private void OnDisable()
        {
            lastSoal.Completed -= Save;
        }

        private void Save()
        {
            if (PlayerSession.HasPlayer) ResultFile.AppendCurrentPlayer();
        }
    }
}
