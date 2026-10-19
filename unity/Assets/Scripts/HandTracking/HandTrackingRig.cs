using DEMSIFT.Core;
using Mediapipe.Unity.Sample;
using UnityEngine;

// Objek ROOT hand tracking (Solution + Main Canvas MediaPipe, CursorCanvas).
// - Dibuat otomatis oleh HandTrackingSpawner saat game mulai, JANGAN ditaruh manual di scene Ko Bryan.
// - Tidak dihancurkan saat pindah scene, jadi cursor tetap ada dari MainMenu sampai soal terakhir.
// - Mengikuti tombol HandTrackingToggle buatan Ko Bryan (HandTrackingSetting).
// - Kalau kamera mati setelah pindah scene, MediaPipe dinyalakan ulang otomatis.
namespace DEMSIFT.HandTracking
{
  public class HandTrackingRig : MonoBehaviour
  {
    [SerializeField] private GameObject cursorCanvas;
    [SerializeField] private HandVirtualMouse virtualMouse;
    [SerializeField] private HandDwellClick dwellClick;

    [Header("Penjaga kamera")]
    [Tooltip("Berapa lama kamera boleh mati sebelum dinyalakan ulang (detik)")]
    [SerializeField] private float restartAfterSeconds = 0.5f;

    private static HandTrackingRig _instance;

    private BaseRunner _runner;
    private bool _cameraWasRunning;
    private float _stoppedTime;

    public static bool Exists => _instance != null;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatic()
    {
      _instance = null;
    }

    private void Awake()
    {
      if (_instance != null && _instance != this)
      {
        Debug.LogWarning("[HandTracking] Ada HandTrackingRig kedua di scene ini dan sudah dihapus. " +
                         "Hapus HandTrackingRig dari scene tersebut, karena rig dibuat otomatis.");
        gameObject.SetActive(false);
        Destroy(gameObject);
        return;
      }
      _instance = this;
      DontDestroyOnLoad(gameObject);
      _runner = GetComponentInChildren<BaseRunner>(true);
    }

    private void OnEnable()
    {
      if (_instance != this) return;
      HandTrackingSetting.Changed += Apply;
      Apply(HandTrackingSetting.IsEnabled);
    }

    private void OnDisable()
    {
      HandTrackingSetting.Changed -= Apply;
    }

    private void OnDestroy()
    {
      if (_instance == this) _instance = null;
    }

    private void Update()
    {
      if (_instance != this || _runner == null) return;

      var source = ImageSourceProvider.ImageSource;
      if (source == null) return;

      if (source.isPlaying)
      {
        _cameraWasRunning = true;
        _stoppedTime = 0f;
        return;
      }

      // Kamera belum pernah jalan (masih menyala pertama kali): tunggu saja.
      if (!_cameraWasRunning) return;

      _stoppedTime += Time.unscaledDeltaTime;
      if (_stoppedTime < restartAfterSeconds) return;

      Debug.Log("[HandTracking] Kamera berhenti (biasanya setelah pindah scene). Menyalakan ulang hand tracking...");
      HandCursorData.Publish(false, 0f, 0f, 1f);
      _cameraWasRunning = false;
      _stoppedTime = 0f;
      _runner.Play();
    }

    private void Apply(bool isEnabled)
    {
      if (cursorCanvas != null) cursorCanvas.SetActive(isEnabled);
      if (virtualMouse != null) virtualMouse.enabled = isEnabled;
      if (dwellClick != null) dwellClick.enabled = isEnabled;
    }
  }
}
