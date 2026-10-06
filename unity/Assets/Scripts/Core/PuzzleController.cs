using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DEMSIFT.Puzzle
{
    public class PuzzleController : MonoBehaviour
    {
        [Header("Data")]
        public PuzzleData data;

        [Header("UI References")]
        public TMP_Text questionLabel;
        public Transform pieceTrayContainer;
        public GameObject piecePrefab;
        public Button nextButton;

        private int correctCount;

        private void Start()
        {
            if (questionLabel != null)
            {
                questionLabel.text = data.questionText;
            }

            nextButton.interactable = false;
            SpawnPieces();
            ArrangePieces();
        }

        // --- Setup ---
        private void SpawnPieces()
        {
            foreach (var piece in data.pieces)
            {
                GameObject pieceObject = Instantiate(piecePrefab, pieceTrayContainer);
                pieceObject.GetComponent<Image>().sprite = piece.pieceSprite;

                TMP_Text caption = pieceObject.GetComponentInChildren<TMP_Text>();
                if (caption != null)
                {
                    caption.text = piece.pieceId;
                }

                DraggableItem draggable = pieceObject.GetComponent<DraggableItem>();
                draggable.pieceId = piece.pieceId;
                draggable.correctZoneId = piece.correctZoneId;
            }
        }

        private void ArrangePieces()
        {
            if (pieceTrayContainer.TryGetComponent(out TrayArranger arranger))
            {
                arranger.Arrange();
            }
        }

        // --- Called by DropZone ---
        public void NotifyCorrectPlacement()
        {
            correctCount++;
            if (correctCount >= data.pieces.Count)
            {
                nextButton.interactable = true;
            }
        }
    }
}
