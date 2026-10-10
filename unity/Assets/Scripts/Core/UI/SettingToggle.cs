using System;
using UnityEngine;
using UnityEngine.UI;

namespace DEMSIFT.UI
{
    [RequireComponent(typeof(Button))]
    public abstract class SettingToggle : MonoBehaviour
    {
        [SerializeField] private GameObject disabledMark;

        private Button button;

        protected abstract bool IsEnabled { get; }

        protected abstract void ToggleSetting();

        protected abstract void Subscribe(Action<bool> handler);

        protected abstract void Unsubscribe(Action<bool> handler);

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            button.onClick.AddListener(ToggleSetting);
            Subscribe(Refresh);
            Refresh(IsEnabled);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(ToggleSetting);
            Unsubscribe(Refresh);
        }

        private void Refresh(bool isEnabled)
        {
            disabledMark.SetActive(!isEnabled);
        }
    }
}
