using UnityEngine;
using UnityEngine.UI;

namespace DEMSIFT.Core
{
    [DefaultExecutionOrder(1000)]
    public class ButtonClickSound : MonoBehaviour
    {
        private void Start()
        {
            foreach (Button button in FindObjectsByType<Button>(FindObjectsInactive.Include))
            {
                button.onClick.AddListener(PlayClick);
            }

            foreach (Toggle toggle in FindObjectsByType<Toggle>(FindObjectsInactive.Include))
            {
                toggle.onValueChanged.AddListener(OnToggleChanged);
            }
        }

        private static void OnToggleChanged(bool isOn)
        {
            if (isOn) PlayClick();
        }

        private static void PlayClick()
        {
            AudioPlayer.PlaySound(SoundEffect.Click);
        }
    }
}
