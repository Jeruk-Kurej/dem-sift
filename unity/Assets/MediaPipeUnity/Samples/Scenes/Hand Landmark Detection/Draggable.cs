using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
  public static readonly List<Draggable> All = new List<Draggable>();

  public string id;
  [HideInInspector] public bool locked;

  public RectTransform Rect => (RectTransform)transform;
  private Vector2 _homePos;

  private void Awake() { _homePos = Rect.anchoredPosition; }
  private void OnEnable() { All.Add(this); }
  private void OnDisable() { All.Remove(this); }

  public void ReturnHome() { Rect.anchoredPosition = _homePos; }
}