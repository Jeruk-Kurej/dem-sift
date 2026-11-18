using UnityEngine;

namespace DEMSIFT.Training
{
    [RequireComponent(typeof(RectTransform))]
    public class BrushCursor : MonoBehaviour
    {
        [SerializeField] private BrushInput input;
        [SerializeField] private float idleScale = 0.9f;
        [SerializeField] private float brushingScale = 1.05f;
        [SerializeField] private float scaleSpeed = 12f;

        private RectTransform rectTransform;
        private RectTransform parentRect;

        private void Awake()
        {
            rectTransform = (RectTransform)transform;
            parentRect = (RectTransform)transform.parent;
        }

        private void OnEnable()
        {
            Cursor.visible = false;
        }

        private void OnDisable()
        {
            Cursor.visible = true;
        }

        private void LateUpdate()
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, input.ScreenPosition, null, out Vector2 point);
            rectTransform.anchoredPosition = point;

            float target = input.IsBrushing ? brushingScale : idleScale;
            float scale = Mathf.Lerp(rectTransform.localScale.x, target, scaleSpeed * Time.unscaledDeltaTime);
            rectTransform.localScale = Vector3.one * scale;
        }
    }
}
