using UnityEngine;
using UnityEngine.UI;

namespace DEMSIFT.HandTracking
{
    public class HandCursor : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private bool mirrorX = true;
        [Tooltip("Smoothing while the hand is still, in Hz. Lower is steadier but slower to settle")]
        [SerializeField, Range(0.1f, 5f)] private float minCutoff = 1f;
        [Tooltip("How quickly smoothing drops while the hand moves. Higher follows fast moves with less delay")]
        [SerializeField, Range(0f, 30f)] private float beta = 8f;

        [Header("Pinch")]
        [Tooltip("Pinch starts when the finger distance ratio drops below this value")]
        [SerializeField] private float pinchOn = 0.25f;
        [Tooltip("Pinch ends when the finger distance ratio rises above this value")]
        [SerializeField] private float pinchOff = 0.4f;

        private readonly OneEuroFilter filter = new OneEuroFilter();
        private Graphic graphic;
        private Vector2 screenPosition;
        private bool hasHand;
        private bool isPinching;

        public Vector2 ScreenPosition => screenPosition;
        public bool HasHand => hasHand;
        public bool IsPinching => isPinching;

        private void Awake()
        {
            graphic = GetComponent<Graphic>();
        }

        private void Update()
        {
            if (!HandCursorData.TryGet(out float x, out float y, out float pinchRatio))
            {
                hasHand = false;
                isPinching = false;
                filter.Reset();
                SetVisible(false);
                return;
            }

            UpdatePosition(x, y);
            UpdatePinch(pinchRatio);
        }

        // --- Position ---
        private void UpdatePosition(float x, float y)
        {
            if (mirrorX) x = 1f - x;

            Vector2 sample = new Vector2(Mathf.Clamp01(x), 1f - Mathf.Clamp01(y));
            Vector2 smoothed = filter.Filter(sample, Time.unscaledDeltaTime, minCutoff, beta);
            screenPosition = new Vector2(smoothed.x * Screen.width, smoothed.y * Screen.height);
            hasHand = true;

            transform.position = new Vector3(screenPosition.x, screenPosition.y, 0f);
            SetVisible(true);
        }

        // --- Pinch ---
        private void UpdatePinch(float pinchRatio)
        {
            if (!isPinching && pinchRatio < pinchOn)
            {
                isPinching = true;
            }
            else if (isPinching && pinchRatio > pinchOff)
            {
                isPinching = false;
            }
        }

        private void SetVisible(bool isVisible)
        {
            if (graphic != null) graphic.enabled = isVisible;
        }
    }
}
