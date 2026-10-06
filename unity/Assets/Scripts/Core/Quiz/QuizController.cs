using System.Collections.Generic;
using DEMSIFT.Scoring;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DEMSIFT.Quiz
{
    public class QuizController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private QuestionData question;

        [Header("UI References")]
        [SerializeField] private TMP_Text questionLabel;
        [SerializeField] private Transform answerButtonContainer;
        [SerializeField] private Button answerButtonPrefab;
        [SerializeField] private Button submitButton;
        [SerializeField] private Color selectedColor = new(1f, 0.85f, 0.3f);

        [Header("Result")]
        [SerializeField] private SoalScore soalScore;
        [SerializeField] private ResultPopup resultPopup;

        private readonly List<Button> answerButtons = new();
        private int selectedIndex = -1;

        private void Start()
        {
            questionLabel.text = question.questionText;
            SpawnAnswerButtons();

            submitButton.interactable = false;
            submitButton.onClick.AddListener(Submit);
        }

        // --- Setup ---
        private void SpawnAnswerButtons()
        {
            bool usesImages = question.optionImages != null && question.optionImages.Length == question.options.Length;

            for (int i = 0; i < question.options.Length; i++)
            {
                int optionIndex = i;
                Button button = Instantiate(answerButtonPrefab, answerButtonContainer);

                TMP_Text label = button.GetComponentInChildren<TMP_Text>();
                Image icon = button.transform.Find("AnswerImage")?.GetComponent<Image>();

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

                button.onClick.AddListener(() => Select(optionIndex));
                answerButtons.Add(button);
            }
        }

        // --- Answering ---
        private void Select(int optionIndex)
        {
            selectedIndex = optionIndex;
            submitButton.interactable = true;

            for (int i = 0; i < answerButtons.Count; i++)
            {
                answerButtons[i].image.color = i == selectedIndex ? selectedColor : Color.white;
            }
        }

        private void Submit()
        {
            if (selectedIndex < 0) return;

            if (selectedIndex == question.correctAnswerIndex)
            {
                soalScore.Complete();
            }
            else
            {
                resultPopup.ShowWrong(soalScore.RegisterWrongAttempt());
            }
        }
    }
}
