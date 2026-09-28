using UnityEngine;
using UnityEngine.EventSystems;

namespace DEMSIFT.Puzzle
{
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public string pieceId;
        public string correctZoneId;

        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        private Canvas rootCanvas;
        private Transform startParent;
        private Vector2 startAnchoredPosition;
        private bool isPlaced;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            rootCanvas = GetComponentInParent<Canvas>();
        }

        // --- Drag lifecycle ---
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (isPlaced) return;

            startParent = transform.parent;
            startAnchoredPosition = rectTransform.anchoredPosition;
            canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isPlaced) return;

            rectTransform.anchoredPosition += eventData.delta / rootCanvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (isPlaced) return;

            canvasGroup.blocksRaycasts = true;

            // If OnDrop on a valid DropZone didn't lock it in place, snap back to tray
            if (transform.parent == startParent)
            {
                rectTransform.anchoredPosition = startAnchoredPosition;
            }
        }

        // --- Called by DropZone ---
        public void LockIntoZone(Transform zone)
        {
            isPlaced = true;
            transform.SetParent(zone, false);
            rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
        }

        public void ReturnToTray()
        {
            transform.SetParent(startParent);
            rectTransform.anchoredPosition = startAnchoredPosition;
        }
    }
}
