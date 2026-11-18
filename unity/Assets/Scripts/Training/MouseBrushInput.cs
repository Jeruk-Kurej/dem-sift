using UnityEngine;
using UnityEngine.InputSystem;

namespace DEMSIFT.Training
{
    public class MouseBrushInput : BrushInput
    {
        public override bool IsBrushing => Mouse.current != null && Mouse.current.leftButton.isPressed;

        public override Vector2 ScreenPosition => Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
    }
}
