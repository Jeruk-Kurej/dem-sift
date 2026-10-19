using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DEMSIFT.HandTracking
{
    public class HandDwellClick : MonoBehaviour
    {
        [SerializeField] private HandCursor handCursor;
        [SerializeField] private Image progressRing;
        [SerializeField] private float holdSeconds = 1.5f;
        [Tooltip("How long the cursor may leave the target (a shaky hand) before the hold restarts")]
        [SerializeField] private float leaveGraceSeconds = 0.3f;

        private readonly List<RaycastResult> hits = new List<RaycastResult>();
        private GameObject target;
        private GameObject justClicked;
        private RaycastResult lastHit;
        private float holdTimer;
        private float leaveTimer;

        private void OnDisable()
        {
            ResetDwell();
        }

        private void Update()
        {
            EventSystem eventSystem = EventSystem.current;
            if (eventSystem == null || handCursor == null || !handCursor.HasHand || handCursor.IsPinching)
            {
                ResetDwell();
                return;
            }

            PointerEventData pointer = new PointerEventData(eventSystem)
            {
                position = handCursor.ScreenPosition,
                button = PointerEventData.InputButton.Left,
            };

            if (!TrackTarget(eventSystem, pointer)) return;

            if (target == null || target == justClicked)
            {
                SetRing(0f);
                return;
            }

            holdTimer += Time.unscaledDeltaTime;
            SetRing(holdTimer / holdSeconds);

            if (holdTimer >= holdSeconds) Click(pointer);
        }

        // --- Target ---
        private bool TrackTarget(EventSystem eventSystem, PointerEventData pointer)
        {
            GameObject hovered = FindClickable(eventSystem, pointer);

            if (hovered != null && hovered == target)
            {
                leaveTimer = 0f;
                lastHit = hits[0];
            }
            else if (hovered == null && target != null)
            {
                leaveTimer += Time.unscaledDeltaTime;
                if (leaveTimer < leaveGraceSeconds) return false;

                target = null;
                justClicked = null;
                holdTimer = 0f;
            }
            else if (hovered != target)
            {
                target = hovered;
                justClicked = null;
                holdTimer = 0f;
                leaveTimer = 0f;
                if (hovered != null) lastHit = hits[0];
            }

            return true;
        }

        private GameObject FindClickable(EventSystem eventSystem, PointerEventData pointer)
        {
            hits.Clear();
            eventSystem.RaycastAll(pointer, hits);
            if (hits.Count == 0) return null;

            GameObject clickable = ExecuteEvents.GetEventHandler<IPointerClickHandler>(hits[0].gameObject);
            if (clickable != null && clickable.TryGetComponent(out Selectable selectable) && !selectable.IsInteractable())
            {
                return null;
            }

            return clickable;
        }

        // --- Click ---
        private void Click(PointerEventData pointer)
        {
            pointer.pointerCurrentRaycast = lastHit;
            pointer.pointerPressRaycast = lastHit;
            pointer.rawPointerPress = lastHit.gameObject;
            pointer.pointerPress = target;
            pointer.clickCount = 1;
            pointer.clickTime = Time.unscaledTime;
            ExecuteEvents.Execute(target, pointer, ExecuteEvents.pointerClickHandler);

            justClicked = target;
            holdTimer = 0f;
            SetRing(0f);
        }

        private void ResetDwell()
        {
            target = null;
            justClicked = null;
            holdTimer = 0f;
            leaveTimer = 0f;
            SetRing(0f);
        }

        private void SetRing(float value)
        {
            if (progressRing != null) progressRing.fillAmount = Mathf.Clamp01(value);
        }
    }
}
