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
        }

        // --- Setup ---
        private void SpawnPieces()
        {
            foreach (var piece in data.pieces)
            {
                GameObject pieceObject = Instantiate(piecePrefab, pieceTrayContainer);
                pieceObject.GetComponent<Image>().sprite = piece.pieceSprite;

                DraggableItem draggable = pieceObject.GetComponent<DraggableItem>();
                draggable.pieceId = piece.pieceId;
                draggable.correctZoneId = piece.correctZoneId;
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
