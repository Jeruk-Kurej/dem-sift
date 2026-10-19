using UnityEngine;
using UnityEngine.UI;

public class HandCursor : MonoBehaviour
{
  [Header("Movement")]
  [SerializeField] private bool mirrorX = true;
  [SerializeField, Range(1f, 30f)] private float smoothing = 12f;

  [Header("Hold to select")]
  [SerializeField] private Image progressRing;
  [SerializeField] private float holdSeconds = 3f;

  [Header("Pinch to drag")]
  [Tooltip("Pinch aktif kalau rasio di bawah angka ini")]
  [SerializeField] private float pinchOn = 0.25f;
  [Tooltip("Pinch lepas kalau rasio di atas angka ini")]
  [SerializeField] private float pinchOff = 0.4f;

  private Vector2 _pos;
  private bool _initialized;
  private Graphic _graphic;

  private HoldSelectable _current;
  private HoldSelectable _justSelected;
  private float _timer;

  private bool _pinching;
  private Draggable _dragged;
  private Vector2 _grabOffset;

  public Vector2 ScreenPosition => _pos;
  public bool IsPinching => _pinching;
  public bool HasHand => _initialized;

  private void Awake()
  {
    _graphic = GetComponent<Graphic>();
    SetRing(0f);
  }

  private void Update()
  {
    if (!HandCursorData.TryGet(out var x, out var y, out var pinchRatio))
    {
      _initialized = false;
      if (_graphic != null) _graphic.enabled = false;
      CancelDrag();
      ResetHold();
      return;
    }

    if (mirrorX) x = 1f - x;

    var target = new Vector2(
      Mathf.Clamp01(x) * Screen.width,
      (1f - Mathf.Clamp01(y)) * Screen.height);

    if (!_initialized)
    {
      _pos = target;
      _initialized = true;
    }
    else
    {
      _pos = Vector2.Lerp(_pos, target, 1f - Mathf.Exp(-smoothing * Time.unscaledDeltaTime));
    }

    transform.position = new Vector3(_pos.x, _pos.y, 0f);
    if (_graphic != null) _graphic.enabled = true;

    if (!_pinching && pinchRatio < pinchOn)
    {
      _pinching = true;
      OnPinchStart();
    }
    else if (_pinching && pinchRatio > pinchOff)
    {
      _pinching = false;
      OnPinchEnd();
    }

    if (_dragged != null)
    {
      var p = _pos + _grabOffset;
      _dragged.transform.position = new Vector3(p.x, p.y, _dragged.transform.position.z);
    }

    if (_pinching)
    {
      ResetHold();
    }
    else
    {
      UpdateHold();
    }
  }

  // ---------- Drag ----------

  private void OnPinchStart()
  {
    Draggable best = null;
    foreach (var d in Draggable.All)
    {
      if (d == null || d.locked || !d.isActiveAndEnabled) continue;
      if (!RectTransformUtility.RectangleContainsScreenPoint(d.Rect, _pos, null)) continue;
      if (best == null || d.transform.GetSiblingIndex() > best.transform.GetSiblingIndex()) best = d;
    }

    _dragged = best;
    if (_dragged != null)
    {
      _grabOffset = (Vector2)_dragged.transform.position - _pos;
      _dragged.transform.SetAsLastSibling();
      transform.SetAsLastSibling();
    }
  }

  private void OnPinchEnd()
  {
    if (_dragged == null) return;

    Vector2 itemPos = _dragged.transform.position;
    DropZone hit = null;
    foreach (var z in DropZone.All)
    {
      if (z == null || !z.isActiveAndEnabled) continue;
      if (RectTransformUtility.RectangleContainsScreenPoint(z.Rect, itemPos, null))
      {
        hit = z;
        break;
      }
    }

    if (hit == null || (hit.filled && !hit.allowMultiple))
    {
      _dragged.ReturnHome();
    }
    else if (hit.acceptedId == _dragged.id)
    {
      if (!hit.allowMultiple)
      {
        _dragged.transform.position = hit.transform.position;
        hit.filled = true;
      }
      _dragged.locked = true;
      hit.onCorrect.Invoke();
    }
    else
    {
      _dragged.ReturnHome();
      hit.onWrong.Invoke();
    }

    _dragged = null;
  }

  private void CancelDrag()
  {
    _pinching = false;
    if (_dragged != null)
    {
      _dragged.ReturnHome();
      _dragged = null;
    }
  }

  // ---------- Hold to select ----------

  private HoldSelectable FindTarget()
  {
    Vector2 p = transform.position;
    foreach (var s in HoldSelectable.All)
    {
      if (s != null && s.isActiveAndEnabled &&
          RectTransformUtility.RectangleContainsScreenPoint(s.Rect, p, null))
        return s;
    }
    return null;
  }

  private void UpdateHold()
  {
    var target = FindTarget();

    if (target != _current)
    {
      SetHover(_current, false);
      _current = target;
      SetHover(_current, true);
      _timer = 0f;
      _justSelected = null;
    }

    if (target == null || target == _justSelected)
    {
      SetRing(0f);
      return;
    }

    _timer += Time.unscaledDeltaTime;
    SetRing(_timer / holdSeconds);

    if (_timer >= holdSeconds)
    {
      _justSelected = target;
      _timer = 0f;
      SetRing(0f);
      target.Select();
    }
  }

  private void ResetHold()
  {
    SetHover(_current, false);
    _current = null;
    _justSelected = null;
    _timer = 0f;
    SetRing(0f);
  }

  private static void SetHover(HoldSelectable s, bool on)
  {
    if (s != null) s.SetHover(on);
  }

  private void SetRing(float value)
  {
    if (progressRing != null) progressRing.fillAmount = Mathf.Clamp01(value);
  }
}