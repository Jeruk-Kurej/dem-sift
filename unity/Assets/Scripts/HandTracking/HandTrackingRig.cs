using DEMSIFT.Core;
using Mediapipe.Unity;
using Mediapipe.Unity.Sample;
using UnityEngine;

namespace DEMSIFT.HandTracking
{
    public class HandTrackingRig : MonoBehaviour
    {
        [SerializeField] private GameObject cursorCanvas;
        [SerializeField] private HandVirtualMouse virtualMouse;
        [SerializeField] private HandDwellClick dwellClick;

        [Header("Camera watchdog")]
        [Tooltip("Seconds the camera may stay stopped before hand tracking restarts")]
        [SerializeField] private float restartAfterSeconds = 0.5f;

        private static HandTrackingRig instance;

        private BaseRunner runner;
        private bool cameraWasRunning;
        private float stoppedTime;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.LogWarning("[HandTracking] A second HandTrackingRig was found and removed. The rig is spawned automatically, so remove it from the scene.");
                gameObject.SetActive(false);
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            runner = GetComponentInChildren<BaseRunner>(true);
        }

        private void OnEnable()
        {
            if (instance != this) return;

            HandTrackingSetting.Changed += Apply;
            Apply(HandTrackingSetting.IsEnabled);
        }

        private void OnDisable()
        {
            HandTrackingSetting.Changed -= Apply;
        }

        private void OnDestroy()
        {
            if (instance == this) instance = null;
        }

        private void Update()
        {
            if (instance != this || runner == null) return;

            WatchCamera();
        }

        // --- Camera watchdog ---
        private void WatchCamera()
        {
            ImageSource source = ImageSourceProvider.ImageSource;
            if (source == null) return;

            if (source.isPlaying)
            {
                cameraWasRunning = true;
                stoppedTime = 0f;
                return;
            }

            if (!cameraWasRunning) return;

            stoppedTime += Time.unscaledDeltaTime;
            if (stoppedTime < restartAfterSeconds) return;

            Debug.Log("[HandTracking] The camera stopped, restarting hand tracking.");
            HandCursorData.Publish(false, 0f, 0f, 1f);
            cameraWasRunning = false;
            stoppedTime = 0f;
            runner.Play();
        }

        // --- Setting ---
        private void Apply(bool isEnabled)
        {
            if (cursorCanvas != null) cursorCanvas.SetActive(isEnabled);
            if (virtualMouse != null) virtualMouse.enabled = isEnabled;
            if (dwellClick != null) dwellClick.enabled = isEnabled;
        }

        // --- Play mode without domain reload ---
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Clear()
        {
            instance = null;
        }
    }
}
