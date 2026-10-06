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
            DraggableItem item = eventData.pointerDrag != null
                ? eventData.pointerDrag.GetComponent<DraggableItem>()
                : null;

            if (item == null || item.IsPlaced) return;

            if (filledCount < capacity && item.correctZoneId == zoneId)
            {
                item.LockIntoZone(transform);
                filledCount++;
                puzzleController.NotifyCorrectPlacement();
            }
            else
            {
                Vector3 dropPosition = item.transform.position;
                item.ReturnToTray();
                puzzleController.NotifyWrongPlacement(dropPosition);
            }
        }
    }
}
