using DEMSIFT.Scoring;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DEMSIFT.Puzzle
{
    public class PuzzleController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private PuzzleData data;

        [Header("UI References")]
        [SerializeField] private TMP_Text questionLabel;
        [SerializeField] private Transform pieceTrayContainer;
        [SerializeField] private GameObject piecePrefab;

        [Header("Result")]
        [SerializeField] private SoalScore soalScore;
        [SerializeField] private PenaltyFeedback penaltyFeedback;

        private int correctCount;

        private void Start()
        {
            if (questionLabel != null)
            {
                questionLabel.text = data.questionText;
            }

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
                    caption.text = string.IsNullOrEmpty(piece.label) ? piece.pieceId : piece.label;
                }

                pieceObject.GetComponent<DraggableItem>().Setup(piece.correctZoneId);
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
                soalScore.Complete();
            }
        }

        public void NotifyWrongPlacement(Vector3 dropPosition)
        {
            penaltyFeedback.Show(soalScore.RegisterWrongAttempt(), dropPosition);
        }
    }
}
