using UnityEngine;
using UnityEngine.EventSystems;

namespace DEMSIFT.Puzzle
{
    public class DropZone : MonoBehaviour, IDropHandler
    {
        public string zoneId;
        public PuzzleController puzzleController;

        private bool isFilled;

        public void OnDrop(PointerEventData eventData)
        {
            if (isFilled) return;

            DraggableItem item = eventData.pointerDrag != null
                ? eventData.pointerDrag.GetComponent<DraggableItem>()
                : null;

            if (item == null) return;

            if (item.correctZoneId == zoneId)
            {
                item.LockIntoZone(transform);
                isFilled = true;
                puzzleController.NotifyCorrectPlacement();
            }
            else
            {
                item.ReturnToTray();
            }
        }
    }
}
