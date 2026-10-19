using UnityEngine;
using UnityEngine.UI;

namespace DEMSIFT.HandTracking
{
    public class HandCursor : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private bool mirrorX = true;
        [SerializeField, Range(1f, 30f)] private float smoothing = 12f;

        [Header("Pinch")]
        [Tooltip("Pinch starts when the finger distance ratio drops below this value")]
        [SerializeField] private float pinchOn = 0.25f;
        [Tooltip("Pinch ends when the finger distance ratio rises above this value")]
        [SerializeField] private float pinchOff = 0.4f;

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

            Vector2 target = new Vector2(Mathf.Clamp01(x) * Screen.width, (1f - Mathf.Clamp01(y)) * Screen.height);
            screenPosition = hasHand
                ? Vector2.Lerp(screenPosition, target, 1f - Mathf.Exp(-smoothing * Time.unscaledDeltaTime))
                : target;
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
