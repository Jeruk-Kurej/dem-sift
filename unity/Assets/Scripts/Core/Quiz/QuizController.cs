using DEMSIFT.Scoring;
using UnityEngine;
using UnityEngine.UI;

namespace DEMSIFT.Quiz
{
    public class QuizController : MonoBehaviour
    {
        [Header("Questions")]
        [SerializeField] private ChoiceGroup[] groups;
        [SerializeField] private Button submitButton;
        [Tooltip("Clears a wrong choice after Submit so the player sees which row to try again.")]
        [SerializeField] private bool clearWrongChoices;

        [Header("Result")]
        [Tooltip("Optional. Leave empty for a soal without a score.")]
        [SerializeField] private SoalScore soalScore;
        [SerializeField] private ResultPopup resultPopup;

        private void Start()
        {
            foreach (ChoiceGroup group in groups)
            {
                group.Build(() => !resultPopup.IsOpen);
                group.SelectionChanged += RefreshSubmit;
            }

            submitButton.interactable = false;
            submitButton.onClick.AddListener(Submit);
        }

        // --- Answering ---
        private void RefreshSubmit()
        {
            submitButton.interactable = AllSelected();
        }

        private void Submit()
        {
            if (!AllSelected() || resultPopup.IsOpen) return;

            if (AllCorrect())
            {
                if (soalScore != null) soalScore.Complete();
                else resultPopup.ShowCorrect();
                return;
            }

            int lostPoints = soalScore != null ? soalScore.RegisterWrongAttempt() : 0;
            if (clearWrongChoices) ClearWrongChoices();
            resultPopup.ShowWrong(lostPoints);
        }

        private void ClearWrongChoices()
        {
            foreach (ChoiceGroup group in groups)
            {
                if (!group.IsCorrect) group.ClearSelection();
            }
        }

        // --- Checks ---
        private bool AllSelected()
        {
            foreach (ChoiceGroup group in groups)
            {
                if (!group.HasSelection) return false;
            }
            return true;
        }

        private bool AllCorrect()
        {
            foreach (ChoiceGroup group in groups)
            {
                if (!group.IsCorrect) return false;
            }
            return true;
        }
    }
}
