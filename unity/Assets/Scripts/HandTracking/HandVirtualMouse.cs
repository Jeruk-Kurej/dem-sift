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
        private bool isPressed;

        private void OnEnable()
        {
            if (mouse == null) mouse = InputSystem.AddDevice<Mouse>("HandMouse");
            hasLastPosition = false;
            isPressed = false;
        }

        private void OnDisable()
        {
            if (isPressed) Send(lastPosition, false);
        }

        private void OnDestroy()
        {
            if (mouse != null && mouse.added) InputSystem.RemoveDevice(mouse);
            mouse = null;
        }

        private void Update()
        {
            if (mouse == null || handCursor == null) return;

            if (!handCursor.HasHand)
            {
                if (isPressed) Send(lastPosition, false);
                return;
            }

            Send(handCursor.ScreenPosition, handCursor.IsPinching);
        }

        // --- Device state ---
        private void Send(Vector2 position, bool pressed)
        {
            MouseState state = new MouseState
            {
                position = position,
                delta = hasLastPosition ? position - lastPosition : Vector2.zero,
            }.WithButton(MouseButton.Left, pressed);

            InputState.Change(mouse, state);

            lastPosition = position;
            hasLastPosition = true;
            isPressed = pressed;
        }
    }
}
