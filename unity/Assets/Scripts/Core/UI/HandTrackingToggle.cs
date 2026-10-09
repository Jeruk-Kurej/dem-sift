using DEMSIFT.Core;
using UnityEngine;
using UnityEngine.UI;

namespace DEMSIFT.UI
{
    [RequireComponent(typeof(Button))]
    public class HandTrackingToggle : MonoBehaviour
    {
        [SerializeField] private GameObject disabledMark;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            button.onClick.AddListener(HandTrackingSetting.Toggle);
            HandTrackingSetting.Changed += Refresh;
            Refresh(HandTrackingSetting.IsEnabled);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(HandTrackingSetting.Toggle);
            HandTrackingSetting.Changed -= Refresh;
        }

        private void Refresh(bool isEnabled)
        {
            disabledMark.SetActive(!isEnabled);
        }
    }
}
