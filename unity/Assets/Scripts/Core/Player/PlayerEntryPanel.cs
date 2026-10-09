using DEMSIFT.Core;
using DEMSIFT.Scoring;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DEMSIFT.Player
{
    public class PlayerEntryPanel : MonoBehaviour
    {
        [Header("Inputs")]
        [SerializeField] private TMP_InputField nameInput;
        [SerializeField] private ToggleGroup gradeGroup;

        [Header("Navigation")]
        [SerializeField] private Button startButton;
        [SerializeField] private GameObject incompleteHint;
        [SerializeField] private SceneLoader sceneLoader;

        private string targetScene;

        private void Awake()
        {
            nameInput.onValueChanged.AddListener(_ => Refresh());
            ListenTo(gradeGroup);
        }

        // --- Called by buttons ---
        public void Open(string sceneName)
        {
            targetScene = sceneName;
            gameObject.SetActive(true);
            Refresh();
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void Confirm()
        {
            if (!IsComplete()) return;

            PlayerSession.Begin(nameInput.text, SelectedLabel(gradeGroup));
            ScoreSession.Clear();
            sceneLoader.LoadScene(targetScene);
        }

        // --- Validation ---
        private void ListenTo(ToggleGroup group)
        {
            foreach (Toggle toggle in group.GetComponentsInChildren<Toggle>(true))
            {
                toggle.onValueChanged.AddListener(_ => Refresh());
            }
        }

        private void Refresh()
        {
            bool complete = IsComplete();
            startButton.interactable = complete;
            incompleteHint.SetActive(!complete);
        }

        private bool IsComplete()
        {
            return !string.IsNullOrWhiteSpace(nameInput.text)
                && gradeGroup.AnyTogglesOn();
        }

        private static string SelectedLabel(ToggleGroup group)
        {
            return group.GetFirstActiveToggle().GetComponentInChildren<TMP_Text>().text;
        }
    }
}
