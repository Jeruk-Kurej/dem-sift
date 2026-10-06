using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace DEMSIFT.Admin
{
    public class PinGate : MonoBehaviour
    {
        [SerializeField] private string pin = "12345";
        [SerializeField] private TMP_InputField pinInput;
        [SerializeField] private GameObject wrongPinHint;
        [SerializeField] private UnityEvent unlocked;

        // --- Called by buttons ---
        public void Open()
        {
            pinInput.text = string.Empty;
            wrongPinHint.SetActive(false);
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void Submit()
        {
            if (pinInput.text == pin)
            {
                Close();
                unlocked.Invoke();
            }
            else
            {
                pinInput.text = string.Empty;
                wrongPinHint.SetActive(true);
            }
        }
    }
}
