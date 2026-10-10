using DEMSIFT.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DEMSIFT.Puzzle
{
    public class DropZone : MonoBehaviour, IDropHandler
    {
        [SerializeField] private string zoneId;
        [SerializeField] private int capacity = 1;
        [SerializeField] private PuzzleController puzzleController;

        private int filledCount;

        public void OnDrop(PointerEventData eventData)
        {
            DraggableItem item = eventData.pointerDrag != null
                ? eventData.pointerDrag.GetComponent<DraggableItem>()
                : null;

            if (item == null || item.IsPlaced) return;

            if (filledCount < capacity && item.CorrectZoneId == zoneId)
            {
                item.LockIntoZone(transform);
                filledCount++;
                AudioPlayer.PlaySound(SoundEffect.Correct);
                puzzleController.NotifyCorrectPlacement();
            }
            else
            {
                Vector3 dropPosition = item.transform.position;
                AudioPlayer.PlaySound(SoundEffect.WrongDrop);
                item.ReturnToTray();
                puzzleController.NotifyWrongPlacement(dropPosition);
            }
        }
    }
}
