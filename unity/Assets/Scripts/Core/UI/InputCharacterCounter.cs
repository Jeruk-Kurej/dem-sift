using TMPro;
using UnityEngine;

namespace DEMSIFT.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class InputCharacterCounter : MonoBehaviour
    {
        [SerializeField] private TMP_InputField input;

        private TMP_Text label;

        private void Awake()
        {
            label = GetComponent<TMP_Text>();
            input.onValueChanged.AddListener(Show);
            Show(input.text);
        }

        private void Show(string value)
        {
            label.text = $"{value.Length}/{input.characterLimit}";
        }
    }
}
