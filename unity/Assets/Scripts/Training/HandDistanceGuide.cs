using TMPro;
using UnityEngine;

namespace DEMSIFT.Training
{
    public class HandDistanceGuide : MonoBehaviour
    {
        [SerializeField] private HandBrushInput input;
        [SerializeField] private TMP_Text label;

        [Header("Palm size")]
        [SerializeField] private float tooFarBelow = 0.08f;
        [SerializeField] private float tooCloseAbove = 0.35f;
        [SerializeField] private float noHandDelay = 0.5f;

        [Header("Text")]
        [SerializeField] private string noHandText = "Angkat tanganmu ke depan kamera ya!";
        [SerializeField] private string tooFarText = "Dekatkan tanganmu ke kamera ya!";
        [SerializeField] private string tooCloseText = "Mundur sedikit ya!";

        [Header("Testing")]
        [SerializeField] private bool showNumbers;

        private float noHandTime;

        private void Update()
        {
            if (input == null || label == null) return;

            if (!input.UsingHand)
            {
                label.text = string.Empty;
                return;
            }

            noHandTime = input.HasHand ? 0f : noHandTime + Time.unscaledDeltaTime;
            string message = Message();

            if (showNumbers && input.HasHand)
            {
                string numbers = $"telapak {input.PalmSize:0.00}  skala {input.MotionScale:0.00}";
                message = string.IsNullOrEmpty(message) ? numbers : message + "\n" + numbers;
            }

            label.text = message;
        }

        private string Message()
        {
            if (!input.HasHand) return noHandTime >= noHandDelay ? noHandText : string.Empty;
            if (input.PalmSize < tooFarBelow) return tooFarText;
            if (input.PalmSize > tooCloseAbove) return tooCloseText;
            return string.Empty;
        }
    }
}
