using UnityEngine;
using UnityEngine.UI;

namespace DEMSIFT.UI
{
    [RequireComponent(typeof(ScrollRect))]
    public class BackToTopVisibility : MonoBehaviour
    {
        [SerializeField] private GameObject button;
        [SerializeField] private float showAfterDistance = 60f;

        private ScrollRect scrollRect;

        private void Awake()
        {
            scrollRect = GetComponent<ScrollRect>();
        }

        private void OnEnable()
        {
            scrollRect.onValueChanged.AddListener(Refresh);
            Refresh(Vector2.zero);
        }

        private void OnDisable()
        {
            scrollRect.onValueChanged.RemoveListener(Refresh);
        }

        private void Refresh(Vector2 scrollPosition)
        {
            button.SetActive(scrollRect.content.anchoredPosition.y > showAfterDistance);
        }
    }
}
