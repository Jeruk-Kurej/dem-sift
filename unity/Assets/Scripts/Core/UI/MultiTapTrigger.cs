using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace DEMSIFT.UI
{
    public class MultiTapTrigger : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private int requiredTaps = 5;
        [SerializeField] private float windowSeconds = 2f;
        [SerializeField] private UnityEvent triggered;

        private int tapCount;
        private float firstTapTime;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (tapCount == 0 || Time.unscaledTime - firstTapTime > windowSeconds)
            {
                tapCount = 0;
                firstTapTime = Time.unscaledTime;
            }

            tapCount++;
            if (tapCount < requiredTaps) return;

            tapCount = 0;
            triggered.Invoke();
        }
    }
}
