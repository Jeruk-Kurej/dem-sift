using TMPro;
using UnityEngine;

namespace DEMSIFT.Scoring
{
    public class ResultPopup : MonoBehaviour
    {
        [Header("Text")]
        [SerializeField] private TMP_Text messageLabel;
        [SerializeField] private TMP_Text scoreLabel;
        [SerializeField] private string correctMessage = "Hebat! Jawabanmu benar!";
        [SerializeField] private string wrongMessage = "Belum tepat, ayo coba lagi!";

        [Header("Buttons")]
        [SerializeField] private GameObject nextButton;
        [SerializeField] private GameObject retryButton;

        [Header("Icons")]
        [SerializeField] private GameObject correctIcon;
        [SerializeField] private GameObject wrongIcon;

        public void ShowCorrect(int score)
        {
            Show(correctMessage, $"Skor: {score}", true);
        }

        public void ShowWrong(int lostPoints)
        {
            Show(wrongMessage, lostPoints > 0 ? $"-{lostPoints}" : string.Empty, false);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        private void Show(string message, string scoreText, bool isCorrect)
        {
            messageLabel.text = message;
            scoreLabel.text = scoreText;
            nextButton.SetActive(isCorrect);
            retryButton.SetActive(!isCorrect);
            correctIcon.SetActive(isCorrect);
            wrongIcon.SetActive(!isCorrect);
            gameObject.SetActive(true);
        }
    }
}
