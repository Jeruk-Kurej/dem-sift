using System.Collections.Generic;
using DEMSIFT.Puzzle;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DEMSIFT.HandTracking
{
    public class HandCursorHover : MonoBehaviour
    {
        [SerializeField] private HandCursor handCursor;

        private readonly List<RaycastResult> hits = new List<RaycastResult>();

        public bool IsHovering { get; private set; }

        private void Update()
        {
            IsHovering = handCursor.HasHand && !handCursor.IsPinching && IsOverInteractable();
        }

        // --- Detection ---
        private bool IsOverInteractable()
        {
            EventSystem eventSystem = EventSystem.current;
            if (eventSystem == null) return false;

            PointerEventData pointer = new PointerEventData(eventSystem) { position = handCursor.ScreenPosition };
            hits.Clear();
            eventSystem.RaycastAll(pointer, hits);
            if (hits.Count == 0) return false;

            GameObject top = hits[0].gameObject;
            return IsClickable(top) || IsDraggable(top);
        }

        private static bool IsClickable(GameObject target)
        {
            GameObject clickable = ExecuteEvents.GetEventHandler<IPointerClickHandler>(target);
            if (clickable == null) return false;

            return !clickable.TryGetComponent(out Selectable selectable) || selectable.IsInteractable();
        }

        private static bool IsDraggable(GameObject target)
        {
            DraggableItem item = target.GetComponentInParent<DraggableItem>();
            return item != null && !item.IsPlaced;
        }
    }
}
