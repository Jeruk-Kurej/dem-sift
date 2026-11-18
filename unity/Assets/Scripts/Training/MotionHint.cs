using TMPro;
using UnityEngine;

namespace DEMSIFT.Training
{
    public class MotionHint : MonoBehaviour
    {
        [SerializeField] private BrushScrubber scrubber;
        [SerializeField] private TMP_Text label;
        [SerializeField] private float lowScore = 0.3f;
        [SerializeField] private float delay = 2f;
        [SerializeField] private float fadeSpeed = 4f;

        private float struggleTime;
        private float alpha;

        // --- Called by BrushingController ---
        public void SetText(string hint)
        {
            label.text = hint;
            struggleTime = 0f;
            alpha = 0f;
            label.alpha = 0f;
        }

        private void Update()
        {
            bool struggling = scrubber.enabled && scrubber.IsBrushing && scrubber.IsInZone && scrubber.Score < lowScore;
            struggleTime = struggling ? struggleTime + Time.deltaTime : 0f;

            bool scoringWell = scrubber.Score >= lowScore;
            float target = struggleTime >= delay ? 1f : scoringWell ? 0f : alpha;
            alpha = Mathf.MoveTowards(alpha, target, fadeSpeed * Time.deltaTime);
            label.alpha = alpha;
        }
    }
}
