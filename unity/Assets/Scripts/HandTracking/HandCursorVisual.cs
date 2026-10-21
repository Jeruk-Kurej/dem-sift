using UnityEngine;
using UnityEngine.UI;

namespace DEMSIFT.HandTracking
{
    [RequireComponent(typeof(Image))]
    public class HandCursorVisual : MonoBehaviour
    {
        [SerializeField] private HandCursor handCursor;
        [SerializeField] private Color idleColor = new Color(0.49f, 0.36f, 0.85f, 0.35f);
        [SerializeField] private Color pinchColor = new Color(0.49f, 0.36f, 0.85f, 0.85f);
        [SerializeField, Range(0.4f, 1f)] private float pinchScale = 0.7f;
        [SerializeField] private float animationSpeed = 20f;

        private Image image;
        private float scale = 1f;

        private void Awake()
        {
            image = GetComponent<Image>();
        }

        private void LateUpdate()
        {
            if (handCursor == null) return;

            bool isPinching = handCursor.IsPinching;
            float blend = 1f - Mathf.Exp(-animationSpeed * Time.unscaledDeltaTime);

            image.color = Color.Lerp(image.color, isPinching ? pinchColor : idleColor, blend);
            scale = Mathf.Lerp(scale, isPinching ? pinchScale : 1f, blend);
            transform.localScale = new Vector3(scale, scale, 1f);
        }
    }
}
