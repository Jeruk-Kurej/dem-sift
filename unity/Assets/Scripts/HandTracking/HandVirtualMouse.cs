using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

// Mengubah cursor tangan menjadi mouse virtual.
// Posisi cursor = posisi mouse, pinch = klik kiri ditekan, lepas pinch = klik kiri dilepas.
// Dengan ini, tombol dan drag & drop UI (EventSystem) bisa dipakai pakai tangan
// tanpa mengubah script UI yang sudah ada.
namespace DEMSIFT.HandTracking
{
public class HandVirtualMouse : MonoBehaviour
{
  [SerializeField] private HandCursor handCursor;

  private Mouse _mouse;
  private Vector2 _lastPos;
  private bool _hasLastPos;

  private void OnEnable()
  {
    _mouse = InputSystem.AddDevice<Mouse>("HandMouse");
    _hasLastPos = false;
  }

  private void OnDisable()
  {
    if (_mouse != null && _mouse.added)
    {
      InputSystem.RemoveDevice(_mouse);
    }
    _mouse = null;
  }

  private void Update()
  {
    if (_mouse == null || handCursor == null) return;

    var hasHand = handCursor.HasHand;
    var pos = hasHand ? handCursor.ScreenPosition : (_hasLastPos ? _lastPos : Vector2.zero);
    var pressed = hasHand && handCursor.IsPinching;

    var state = new MouseState
    {
      position = pos,
      delta = _hasLastPos ? pos - _lastPos : Vector2.zero,
    }.WithButton(MouseButton.Left, pressed);

    InputState.Change(_mouse, state);

    _lastPos = pos;
    _hasLastPos = true;
  }
}
}
