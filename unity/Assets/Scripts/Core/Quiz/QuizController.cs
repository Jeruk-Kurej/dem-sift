using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DemSift.Core.Quiz
{
    // Reusable quiz logic. Assign a QuestionData asset in the Inspector,
    // and this generates the answer buttons and checks clicks automatically.
    // Supports both text answers and image answers (see QuestionData.optionImages).
    public class QuizController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private QuestionData question;

        [Header("UI References")]
        [SerializeField] private TMP_Text questionLabel;
        [SerializeField] private Transform answerButtonContainer;
        [SerializeField] private Button answerButtonPrefab;

        private void Start()
        {
            DisplayQuestion();
        }

        private void DisplayQuestion()
        {
            questionLabel.text = question.questionText;

            // Clear any leftover buttons (safety, in case of re-display)
            foreach (Transform child in answerButtonContainer)
            {
                Destroy(child.gameObject);
            }

            bool usesImages = question.optionImages != null && question.optionImages.Length == question.options.Length;

            // Generate one button per answer option, however many there are
            for (int i = 0; i < question.options.Length; i++)
            {
                int optionIndex = i; // local copy, needed so the click callback below captures the right index
                Button newButton = Instantiate(answerButtonPrefab, answerButtonContainer);

                TMP_Text label = newButton.GetComponentInChildren<TMP_Text>();
                Image icon = newButton.transform.Find("AnswerImage")?.GetComponent<Image>();

                if (usesImages && icon != null)
                {
                    icon.sprite = question.optionImages[i];
                    icon.gameObject.SetActive(true);
                    if (label != null) label.gameObject.SetActive(false);
                }
                else
                {
                    if (label != null) label.text = question.options[i];
                    if (icon != null) icon.gameObject.SetActive(false);
                }

                newButton.onClick.AddListener(() => OnAnswerSelected(optionIndex));
            }
        }

        private void OnAnswerSelected(int selectedIndex)
        {
            bool isCorrect = selectedIndex == question.correctAnswerIndex;
            Debug.Log(isCorrect ? "Benar!" : "Salah, coba lagi.");

            // TODO: replace with real feedback later (color change, sound, move to next scene)
        }
    }
}
