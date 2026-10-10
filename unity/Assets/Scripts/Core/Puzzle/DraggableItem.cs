using DEMSIFT.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DEMSIFT.Puzzle
{
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        private Canvas rootCanvas;
        private Transform startParent;
        private Vector2 startAnchoredPosition;
        private bool isPlaced;

        public string CorrectZoneId { get; private set; }
        public bool IsPlaced => isPlaced;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            rootCanvas = GetComponentInParent<Canvas>();
        }

        // --- Called by PuzzleController ---
        public void Setup(string correctZoneId)
        {
            CorrectZoneId = correctZoneId;
        }

        // --- Drag lifecycle ---
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (isPlaced) return;

            AudioPlayer.PlaySound(SoundEffect.Pickup);
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
