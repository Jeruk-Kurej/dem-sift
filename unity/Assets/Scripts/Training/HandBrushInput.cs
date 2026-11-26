using DEMSIFT.Core;
using DEMSIFT.HandTracking;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DEMSIFT.Training
{
    public class HandBrushInput : BrushInput
    {
        [SerializeField] private BrushScrubber scrubber;

        [Header("Movement")]
        [SerializeField] private bool mirrorX = true;
        [SerializeField, Range(0.5f, 1f)] private float reach = 0.8f;
        [SerializeField, Range(0.1f, 5f)] private float minCutoff = 1f;
        [SerializeField, Range(0f, 30f)] private float beta = 8f;

        [Header("Hand size")]
        [SerializeField] private float referencePalmSize = 0.15f;
        [SerializeField] private float minMotionScale = 0.4f;
        [SerializeField] private float maxMotionScale = 2.5f;
        [SerializeField] private float palmSizeSmoothing = 2f;

        private readonly OneEuroFilter filter = new OneEuroFilter();
        private Vector2 screenPosition;
        private bool hasHand;
        private float palmSize;

        public bool UsingHand => HandTrackingSetting.IsEnabled;
        public bool HasHand => hasHand;
        public float PalmSize => palmSize;

        public override bool IsBrushing => UsingHand
            ? hasHand
            : Mouse.current != null && Mouse.current.leftButton.isPressed;

        public override Vector2 ScreenPosition => UsingHand
            ? screenPosition
            : Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;

        public override float MotionScale => UsingHand && palmSize > 0f
            ? Mathf.Clamp(palmSize / referencePalmSize, minMotionScale, maxMotionScale)
            : 1f;

        private void OnDisable()
        {
            SetBrushingMode(false);
            hasHand = false;
            filter.Reset();
        }

        private void Update()
        {
            SetBrushingMode(UsingHand && (scrubber == null || scrubber.enabled));

            if (!UsingHand || !HandCursorData.TryGetPalm(out Vector2 center, out Vector2 axis))
            {
                hasHand = false;
                filter.Reset();
                return;
            }

            UpdatePosition(center);
            UpdatePalmSize(axis);
            hasHand = true;
        }

        private void UpdatePosition(Vector2 center)
        {
            float x = mirrorX ? 1f - center.x : center.x;
            Vector2 sample = new Vector2(Mathf.Clamp01(x), 1f - Mathf.Clamp01(center.y));
            sample = (sample - Vector2.one * 0.5f) / reach + Vector2.one * 0.5f;
            sample = Vector2.Min(Vector2.Max(sample, Vector2.zero), Vector2.one);

            Vector2 smoothed = filter.Filter(sample, Time.unscaledDeltaTime, minCutoff, beta);
            screenPosition = new Vector2(smoothed.x * Screen.width, smoothed.y * Screen.height);
        }

        private void UpdatePalmSize(Vector2 axis)
        {
            float height = Mathf.Max(Screen.height, 1);
            float size = new Vector2(axis.x * Screen.width, axis.y * Screen.height).magnitude / reach / height;

            if (!hasHand || palmSize <= 0f)
            {
                palmSize = size;
                return;
            }

            palmSize = Mathf.Lerp(palmSize, size, 1f - Mathf.Exp(-palmSizeSmoothing * Time.unscaledDeltaTime));
        }

        private static void SetBrushingMode(bool isOn)
        {
            HandCursor.Hidden = isOn;
            HandVirtualMouse.ClickBlocked = isOn;
        }
    }
}
