using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DropZone : MonoBehaviour
{
  public static readonly List<DropZone> All = new List<DropZone>();

  public string acceptedId;
  [Tooltip("Centang untuk keranjang (soal 5): boleh menerima banyak item")]
  public bool allowMultiple;
  public UnityEvent onCorrect;
  public UnityEvent onWrong;

  [HideInInspector] public bool filled;
  public RectTransform Rect => (RectTransform)transform;

  private void OnEnable() { All.Add(this); }
  private void OnDisable() { All.Remove(this); }
}