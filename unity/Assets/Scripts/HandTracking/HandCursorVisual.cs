using UnityEngine;
using UnityEngine.UI;

namespace DEMSIFT.HandTracking
{
    [RequireComponent(typeof(Image))]
    public class HandCursorVisual : MonoBehaviour
    {
        private enum State { Idle, Hover, Pinch }

        [SerializeField] private HandCursor handCursor;
        [SerializeField] private HandCursorHover hover;

        [Header("Sprites")]
        [SerializeField] private Sprite idleSprite;
        [SerializeField] private Sprite hoverSprite;
        [SerializeField] private Sprite pinchSprite;

        [Header("Look")]
        [SerializeField, Range(0f, 1f)] private float idleAlpha = 0.75f;
        [SerializeField] private float idleScale = 0.8f;
        [SerializeField] private float hoverScale = 1.1f;
        [SerializeField] private float pinchScale = 0.75f;

        [Header("Motion")]
        [SerializeField, Range(0f, 0.2f)] private float pulseAmount = 0.05f;
        [SerializeField] private float pulseSpeed = 4f;
        [SerializeField] private float animationSpeed = 20f;

        private Image image;
        private State state = State.Pinch;
        private float scale = 1f;
        private float alpha = 1f;

        private void Awake()
        {
            image = GetComponent<Image>();
        }

        private void LateUpdate()
        {
            if (handCursor == null) return;

            SetState(CurrentState());

            float blend = 1f - Mathf.Exp(-animationSpeed * Time.unscaledDeltaTime);
            scale = Mathf.Lerp(scale, TargetScale(), blend);
            alpha = Mathf.Lerp(alpha, state == State.Idle ? idleAlpha : 1f, blend);

            transform.localScale = new Vector3(scale, scale, 1f);
            image.color = new Color(1f, 1f, 1f, alpha);
        }

        // --- State ---
        private State CurrentState()
        {
            if (handCursor.IsPinching) return State.Pinch;
            return hover != null && hover.IsHovering ? State.Hover : State.Idle;
        }

        private void SetState(State newState)
        {
            if (newState == state) return;

            state = newState;
            image.sprite = state switch
            {
                State.Hover => hoverSprite,
                State.Pinch => pinchSprite,
                _ => idleSprite,
            };
        }

        private float TargetScale()
        {
            float baseScale = state switch
            {
                State.Hover => hoverScale,
                State.Pinch => pinchScale,
                _ => idleScale,
            };

            if (state == State.Pinch) return baseScale;
            return baseScale * (1f + pulseAmount * Mathf.Sin(Time.unscaledTime * pulseSpeed));
        }
    }
}
