using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HoldSelectable : MonoBehaviour
{
  public static readonly List<HoldSelectable> All = new List<HoldSelectable>();

  [SerializeField] private float hoverScale = 1.1f;
  public UnityEvent onSelected;

  public RectTransform Rect => (RectTransform)transform;
  private Vector3 _baseScale;

  private void Awake() { _baseScale = transform.localScale; }
  private void OnEnable() { All.Add(this); }

  private void OnDisable()
  {
    All.Remove(this);
    transform.localScale = _baseScale;
  }

  public void SetHover(bool on)
  {
    transform.localScale = on ? _baseScale * hoverScale : _baseScale;
  }

  public void Select() { onSelected.Invoke(); }
}