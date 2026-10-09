using UnityEngine;
using UnityEngine.EventSystems;

namespace DEMSIFT.UI
{
    public class PressScale : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField] private float pressedScale = 0.9f;

        private Vector3 normalScale;

        private void Awake()
        {
            normalScale = transform.localScale;
        }

        private void OnDisable()
        {
            transform.localScale = normalScale;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            transform.localScale = normalScale * pressedScale;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            transform.localScale = normalScale;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.localScale = normalScale;
        }
    }
}
