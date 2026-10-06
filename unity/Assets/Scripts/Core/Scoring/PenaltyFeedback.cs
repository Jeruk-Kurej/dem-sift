using DEMSIFT.UI;
using UnityEngine;

namespace DEMSIFT.Scoring
{
    public class PenaltyFeedback : MonoBehaviour
    {
        [SerializeField] private FloatingText floatingTextPrefab;
        [SerializeField] private Transform container;

        public void Show(int lostPoints, Vector3 position)
        {
            if (lostPoints <= 0) return;

            FloatingText text = Instantiate(floatingTextPrefab, position, Quaternion.identity, container);
            text.Show($"-{lostPoints}");
        }
    }
}
