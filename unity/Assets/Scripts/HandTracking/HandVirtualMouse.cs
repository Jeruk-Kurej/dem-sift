using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace DEMSIFT.HandTracking
{
    public class HandVirtualMouse : MonoBehaviour
    {
        [SerializeField] private HandCursor handCursor;

        private Mouse mouse;
        private Vector2 lastPosition;
        private bool hasLastPosition;

        private void OnEnable()
        {
            mouse = InputSystem.AddDevice<Mouse>("HandMouse");
            hasLastPosition = false;
        }

        private void OnDisable()
        {
            if (mouse != null && mouse.added) InputSystem.RemoveDevice(mouse);
            mouse = null;
        }

        private void Update()
        {
            if (mouse == null || handCursor == null) return;

            bool hasHand = handCursor.HasHand;
            Vector2 position = hasHand ? handCursor.ScreenPosition : (hasLastPosition ? lastPosition : Vector2.zero);
            bool isPressed = hasHand && handCursor.IsPinching;

            MouseState state = new MouseState
            {
                position = position,
                delta = hasLastPosition ? position - lastPosition : Vector2.zero,
            }.WithButton(MouseButton.Left, isPressed);

            InputState.Change(mouse, state);

            lastPosition = position;
            hasLastPosition = true;
        }
    }
}
