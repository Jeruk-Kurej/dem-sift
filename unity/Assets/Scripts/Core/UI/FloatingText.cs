using TMPro;
using UnityEngine;

namespace DEMSIFT.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class FloatingText : MonoBehaviour
    {
        [SerializeField] private float duration = 1f;
        [SerializeField] private float riseDistance = 120f;

        private TMP_Text label;
        private RectTransform rectTransform;
        private Vector2 startPosition;
        private float elapsed;

        public void Show(string text)
        {
            label = GetComponent<TMP_Text>();
            rectTransform = (RectTransform)transform;
            startPosition = rectTransform.anchoredPosition;
            label.text = text;
        }

        private void Update()
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);

            rectTransform.anchoredPosition = startPosition + Vector2.up * (riseDistance * progress);
            label.alpha = 1f - progress;

            if (progress >= 1f) Destroy(gameObject);
        }
    }
}
