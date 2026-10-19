using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Tunjuk tombol/pilihan dengan cursor tangan dan tahan selama holdSeconds,
// maka elemen itu "diklik" (Button, Toggle, dan apa pun yang bisa diklik di UI).
// Tidak perlu memasang komponen apa pun di UI Ko Bryan.
// Item yang hanya bisa di-drag (DraggableItem) tidak ikut terpilih, karena tidak bisa diklik.
namespace DEMSIFT.HandTracking
{
  public class HandDwellClick : MonoBehaviour
  {
    [SerializeField] private HandCursor handCursor;
    [SerializeField] private Image progressRing;
    [SerializeField] private float holdSeconds = 1.5f;
    [Tooltip("Cursor boleh keluar sebentar dari tombol (karena tangan goyang) tanpa mengulang hitungan")]
    [SerializeField] private float leaveGraceSeconds = 0.3f;

    private readonly List<RaycastResult> _hits = new List<RaycastResult>();
    private GameObject _target;
    private GameObject _justClicked;
    private float _timer;
    private float _lostTimer;
    private RaycastResult _lastHit;

    private void OnDisable()
    {
      ResetDwell();
    }

    private void Update()
    {
      var eventSystem = EventSystem.current;
      if (eventSystem == null || handCursor == null || !handCursor.HasHand || handCursor.IsPinching)
      {
        ResetDwell();
        return;
      }

      var data = new PointerEventData(eventSystem)
      {
        position = handCursor.ScreenPosition,
        button = PointerEventData.InputButton.Left,
      };

      _hits.Clear();
      eventSystem.RaycastAll(data, _hits);

      GameObject target = null;
      if (_hits.Count > 0)
      {
        target = ExecuteEvents.GetEventHandler<IPointerClickHandler>(_hits[0].gameObject);
        if (target != null && target.TryGetComponent<Selectable>(out var selectable) && !selectable.IsInteractable())
        {
          target = null;
        }
      }

      if (target == _target && target != null)
      {
        _lostTimer = 0f;
        _lastHit = _hits[0];
      }
      else if (target == null && _target != null)
      {
        // Tangan goyang keluar sebentar: tahan hitungan dulu, jangan langsung diulang.
        _lostTimer += Time.unscaledDeltaTime;
        if (_lostTimer < leaveGraceSeconds) return;
        _target = null;
        _justClicked = null;
        _timer = 0f;
      }
      else if (target != _target)
      {
        _target = target;
        _justClicked = null;
        _timer = 0f;
        _lostTimer = 0f;
        if (target != null) _lastHit = _hits[0];
      }

      if (_target == null || _target == _justClicked)
      {
        SetRing(0f);
        return;
      }

      _timer += Time.unscaledDeltaTime;
      SetRing(_timer / holdSeconds);

      if (_timer >= holdSeconds)
      {
        data.pointerCurrentRaycast = _lastHit;
        data.pointerPressRaycast = _lastHit;
        data.rawPointerPress = _lastHit.gameObject;
        data.pointerPress = _target;
        data.clickCount = 1;
        data.clickTime = Time.unscaledTime;
        ExecuteEvents.Execute(_target, data, ExecuteEvents.pointerClickHandler);

        _justClicked = _target;
        _timer = 0f;
        SetRing(0f);
      }
    }

    private void ResetDwell()
    {
      _target = null;
      _justClicked = null;
      _timer = 0f;
      _lostTimer = 0f;
      SetRing(0f);
    }

    private void SetRing(float value)
    {
      if (progressRing != null) progressRing.fillAmount = Mathf.Clamp01(value);
    }
  }
}
