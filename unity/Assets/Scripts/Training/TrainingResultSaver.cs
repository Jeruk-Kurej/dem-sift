using DEMSIFT.Data;
using DEMSIFT.Player;
using UnityEngine;

namespace DEMSIFT.Training
{
    public class TrainingResultSaver : MonoBehaviour
    {
        [SerializeField] private BrushingController brushing;

        private void OnEnable()
        {
            brushing.Completed += Save;
        }

        private void OnDisable()
        {
            brushing.Completed -= Save;
        }

        private void Save()
        {
            if (PlayerSession.HasPlayer) ResultFile.Pelatihan.AppendCurrentPlayer();
        }
    }
}
