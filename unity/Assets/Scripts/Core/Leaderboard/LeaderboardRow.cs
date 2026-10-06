using DEMSIFT.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DEMSIFT.Leaderboard
{
    public class LeaderboardRow : MonoBehaviour
    {
        [SerializeField] private TMP_Text rankLabel;
        [SerializeField] private TMP_Text nameLabel;
        [SerializeField] private TMP_Text classLabel;
        [SerializeField] private TMP_Text totalLabel;
        [SerializeField] private Image background;
        [SerializeField] private Color highlightColor = new(1f, 0.85f, 0.3f, 0.9f);
        [SerializeField] private Color highlightTextColor = new(0.1f, 0.15f, 0.25f);

        public void Show(int rank, PlayerResult result, bool isCurrentPlayer)
        {
            rankLabel.text = rank.ToString();
            nameLabel.text = result.PlayerName;
            classLabel.text = result.ClassName;
            totalLabel.text = result.Total.ToString();

            if (isCurrentPlayer) Highlight();
        }

        private void Highlight()
        {
            background.color = highlightColor;
            rankLabel.color = highlightTextColor;
            nameLabel.color = highlightTextColor;
            classLabel.color = highlightTextColor;
            totalLabel.color = highlightTextColor;
        }
    }
}
