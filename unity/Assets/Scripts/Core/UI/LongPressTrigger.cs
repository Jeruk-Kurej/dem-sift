using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace DEMSIFT.UI
{
    public class LongPressTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField] private float holdSeconds = 3f;
        [SerializeField] private UnityEvent longPressed;

        private bool isHolding;
        private float heldSeconds;

        public void OnPointerDown(PointerEventData eventData)
        {
            isHolding = true;
            heldSeconds = 0f;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isHolding = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHolding = false;
        }

        private void Update()
        {
            if (!isHolding) return;

            heldSeconds += Time.unscaledDeltaTime;
            if (heldSeconds < holdSeconds) return;

            isHolding = false;
            longPressed.Invoke();
        }
    }
}
