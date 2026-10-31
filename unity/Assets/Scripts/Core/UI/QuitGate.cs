using UnityEngine;
using UnityEngine.InputSystem;

namespace DEMSIFT.UI
{
    public class QuitGate : MonoBehaviour
    {
        [SerializeField] private GameObject confirmPanel;

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null || !keyboard.escapeKey.wasPressedThisFrame) return;

            confirmPanel.SetActive(!confirmPanel.activeSelf);
        }

        // --- Called by buttons ---
        public void Cancel()
        {
            confirmPanel.SetActive(false);
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
