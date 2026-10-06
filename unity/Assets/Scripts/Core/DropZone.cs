using UnityEngine;
using UnityEngine.EventSystems;

namespace DEMSIFT.Puzzle
{
    public class DropZone : MonoBehaviour, IDropHandler
    {
        public string zoneId;
        public int capacity = 1;
        public PuzzleController puzzleController;

        private int filledCount;

        public void OnDrop(PointerEventData eventData)
        {
            if (filledCount >= capacity) return;

            DraggableItem item = eventData.pointerDrag != null
                ? eventData.pointerDrag.GetComponent<DraggableItem>()
                : null;

            if (item == null) return;

            if (item.correctZoneId == zoneId)
            {
                item.LockIntoZone(transform);
                filledCount++;
                puzzleController.NotifyCorrectPlacement();
            }
            else
            {
                item.ReturnToTray();
            }
        }
    }
}
