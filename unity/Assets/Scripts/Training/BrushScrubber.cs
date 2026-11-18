using System.Collections.Generic;
using UnityEngine;

namespace DEMSIFT.Training
{
    public class BrushScrubber : MonoBehaviour
    {
        [SerializeField] private BrushInput input;
        [SerializeField] private RectTransform zoneLayer;

        [Header("Tuning")]
        [SerializeField] private float motionWindow = 1.2f;
        [SerializeField] private float minimumSpeed = 80f;
        [SerializeField] private float brushRadius = 130f;
        [SerializeField] private float cleanPerSecond = 3f;

        private readonly List<PlaqueZone> zones = new();
        private MotionClassifier classifier;
        private BrushMotion motion;

        public float Score { get; private set; }
        public bool IsInZone { get; private set; }
        public bool IsBrushing => input.IsBrushing;

        private void Awake()
        {
            classifier = new MotionClassifier(motionWindow, minimumSpeed);
        }

        // --- Called by BrushingController ---
        public void Begin(BrushMotion stepMotion, IEnumerable<PlaqueZone> stepZones)
        {
            motion = stepMotion;
            zones.Clear();
            zones.AddRange(stepZones);
            classifier.Clear();
            enabled = true;
        }

        public void Stop()
        {
            enabled = false;
            Score = 0f;
            IsInZone = false;
        }

        // --- Brushing ---
        private void Update()
        {
            if (!input.IsBrushing)
            {
                classifier.Clear();
                Score = 0f;
                IsInZone = false;
                return;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(zoneLayer, input.ScreenPosition, null, out Vector2 point);
            classifier.Add(point, Time.unscaledTime);
            Score = classifier.Score(motion);

            IsInZone = false;
            foreach (PlaqueZone zone in zones)
            {
                if (!zone.Contains(point)) continue;

                IsInZone = true;
                zone.Scrub(point, brushRadius, Score * cleanPerSecond * Time.deltaTime);
            }
        }
    }
}
