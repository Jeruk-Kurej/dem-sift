using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace DEMSIFT.UI
{
    public class QuitGate : MonoBehaviour
    {
        [SerializeField] private GameObject confirmPanel;
        [SerializeField] private GameObject menuButton;
        [SerializeField] private TMP_Text message;
        [SerializeField] private string menuScene = "MainMenu";
        [SerializeField] private string quitMessage = "Keluar dari game?";
        [SerializeField] private string pauseMessage = "Mau berhenti dulu?";

        private bool IsOnMenu => SceneManager.GetActiveScene().name == menuScene;

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null || !keyboard.escapeKey.wasPressedThisFrame) return;

            if (confirmPanel.activeSelf) Cancel();
            else Open();
        }

        // --- Called by buttons ---
        public void Open()
        {
            bool onMenu = IsOnMenu;
            menuButton.SetActive(!onMenu);
            message.text = onMenu ? quitMessage : pauseMessage;
            confirmPanel.SetActive(true);
        }

        public void Cancel()
        {
            confirmPanel.SetActive(false);
        }

        public void BackToMenu()
        {
            confirmPanel.SetActive(false);
            SceneManager.LoadScene(menuScene);
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
