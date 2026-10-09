using TMPro;
using UnityEngine;

namespace DEMSIFT.Scoring
{
    public class ScoreLabel : MonoBehaviour
    {
        [SerializeField] private SoalScore soalScore;

        private TMP_Text label;

        private void Start()
        {
            label = GetComponentInChildren<TMP_Text>();
            soalScore.Changed += Show;
            Show(soalScore.Current);
        }

        private void OnDestroy()
        {
            soalScore.Changed -= Show;
        }

        private void Show(int score)
        {
            label.text = $"Skor: {score}";
        }
    }
}
