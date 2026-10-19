using UnityEngine;

public static class HandCursorData
{
  private static readonly object _lock = new object();
  private static bool _hasHand;
  private static float _x;
  private static float _y;
  private static float _pinch = 1f;

  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
  private static void ResetOnPlay()
  {
    lock (_lock)
    {
      _hasHand = false;
      _x = 0f;
      _y = 0f;
      _pinch = 1f;
    }
  }

  public static void Publish(bool hasHand, float x, float y, float pinchRatio)
  {
    lock (_lock)
    {
      _hasHand = hasHand;
      _x = x;
      _y = y;
      _pinch = pinchRatio;
    }
  }

  public static bool TryGet(out float x, out float y, out float pinchRatio)
  {
    lock (_lock)
    {
      x = _x;
      y = _y;
      pinchRatio = _pinch;
      return _hasHand;
    }
  }
}