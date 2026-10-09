using UnityEngine;

namespace DEMSIFT.UI
{
    public class BobMotion : MonoBehaviour
    {
        [SerializeField] private Vector2 distance = new Vector2(0f, 12f);
        [SerializeField] private float period = 3f;

        private RectTransform rectTransform;
        private Vector2 startPosition;
        private float phase;

        private void Awake()
        {
            rectTransform = (RectTransform)transform;
            startPosition = rectTransform.anchoredPosition;
            phase = Random.value * Mathf.PI * 2f;
        }

        private void Update()
        {
            float wave = Mathf.Sin(Time.time * Mathf.PI * 2f / period + phase);
            rectTransform.anchoredPosition = startPosition + distance * wave;
        }
    }
}
