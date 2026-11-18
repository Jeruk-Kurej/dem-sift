using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DEMSIFT.Leaderboard
{
    public class LeaderboardTab : MonoBehaviour
    {
        [SerializeField] private Image fill;
        [SerializeField] private GameObject border;
        [SerializeField] private TMP_Text label;
        [SerializeField] private Color activeFillColor = new(0.725f, 0.627f, 0.945f, 1f);
        [SerializeField] private Color activeTextColor = Color.white;
        [SerializeField] private Color idleTextColor = new(0.55f, 0.45f, 0.82f, 1f);
        [SerializeField] private Material activeTextMaterial;
        [SerializeField] private Material idleTextMaterial;

        public void SetActive(bool isActive)
        {
            fill.color = isActive ? activeFillColor : Color.clear;
            border.SetActive(isActive);
            label.color = isActive ? activeTextColor : idleTextColor;
            label.fontSharedMaterial = isActive ? activeTextMaterial : idleTextMaterial;
        }
    }
}
