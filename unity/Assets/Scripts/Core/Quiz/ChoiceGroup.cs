using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DEMSIFT.Quiz
{
    public class ChoiceGroup : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private QuestionData question;

        [Header("UI References")]
        [SerializeField] private TMP_Text questionLabel;
        [SerializeField] private Transform answerButtonContainer;
        [SerializeField] private Button answerButtonPrefab;
        [SerializeField] private Color selectedColor = new(1f, 0.85f, 0.3f);
        [Tooltip("Keep the option text under the picture instead of hiding it (image answers only).")]
        [SerializeField] private bool keepLabelWithImage;

        private readonly List<Button> answerButtons = new();
        private Func<bool> selectionAllowed;
        private int selectedIndex = -1;

        public event Action SelectionChanged;

        public bool HasSelection => selectedIndex >= 0;
        public bool IsCorrect => selectedIndex == question.correctAnswerIndex;

        // --- Called by QuizController ---
        public void Build(Func<bool> canSelect)
        {
            selectionAllowed = canSelect;
            questionLabel.text = question.questionText;
            SpawnAnswerButtons();
        }

        public void ClearSelection()
        {
            SetSelection(-1);
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
                    if (label != null)
                    {
                        label.text = question.options[i];
                        label.gameObject.SetActive(keepLabelWithImage);
                    }
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

        // --- Selection ---
        private void Select(int optionIndex)
        {
            if (!selectionAllowed()) return;

            SetSelection(optionIndex);
        }

        private void SetSelection(int optionIndex)
        {
            selectedIndex = optionIndex;

            for (int i = 0; i < answerButtons.Count; i++)
            {
                answerButtons[i].image.color = i == selectedIndex ? selectedColor : Color.white;
            }

            SelectionChanged?.Invoke();
        }
    }
}
