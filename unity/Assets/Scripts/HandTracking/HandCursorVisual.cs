using UnityEngine;
using UnityEngine.UI;

// Tampilan cursor tangan: samar saat diam, lebih tegas dan mengecil saat pinch.
// Sekaligus jadi tanda apakah pinch-mu terbaca atau tidak.
namespace DEMSIFT.HandTracking
{
  [RequireComponent(typeof(Image))]
  public class HandCursorVisual : MonoBehaviour
  {
    [SerializeField] private HandCursor handCursor;
    [SerializeField] private Color idleColor = new Color(0.49f, 0.36f, 0.85f, 0.35f);
    [SerializeField] private Color pinchColor = new Color(0.49f, 0.36f, 0.85f, 0.85f);
    [SerializeField, Range(0.4f, 1f)] private float pinchScale = 0.7f;
    [SerializeField] private float animationSpeed = 20f;

    private Image _image;
    private float _scale = 1f;

    private void Awake()
    {
      _image = GetComponent<Image>();
    }

    private void LateUpdate()
    {
      if (handCursor == null) return;

      var pinching = handCursor.IsPinching;
      var t = 1f - Mathf.Exp(-animationSpeed * Time.unscaledDeltaTime);

      _image.color = Color.Lerp(_image.color, pinching ? pinchColor : idleColor, t);
      _scale = Mathf.Lerp(_scale, pinching ? pinchScale : 1f, t);
      transform.localScale = new Vector3(_scale, _scale, 1f);
    }
  }
}
