using System;
using DEMSIFT.Core;

namespace DEMSIFT.UI
{
    public class SoundToggle : SettingToggle
    {
        protected override bool IsEnabled => AudioSetting.IsEnabled;

        protected override void ToggleSetting()
        {
            AudioSetting.Toggle();
        }

        protected override void Subscribe(Action<bool> handler)
        {
            AudioSetting.Changed += handler;
        }

        protected override void Unsubscribe(Action<bool> handler)
        {
            AudioSetting.Changed -= handler;
        }
    }
}
