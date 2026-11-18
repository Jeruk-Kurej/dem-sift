using System;
using System.Collections;
using System.Collections.Generic;
using DEMSIFT.Core;
using DEMSIFT.Scoring;
using DEMSIFT.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DEMSIFT.Training
{
    public class BrushingController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private BrushStepData[] steps;

        [Header("Scene")]
        [SerializeField] private Image mouthImage;
        [SerializeField] private RectTransform zoneLayer;
        [SerializeField] private PlaqueZone zonePrefab;
        [SerializeField] private BrushScrubber scrubber;
        [SerializeField] private MotionHint hint;

        [Header("Labels")]
        [SerializeField] private TMP_Text instructionLabel;
        [SerializeField] private TMP_Text motionLabel;
        [SerializeField] private TMP_Text progressLabel;

        [Header("Completion")]
        [SerializeField, Range(0.5f, 1f)] private float cleanThreshold = 0.85f;
        [SerializeField] private float finishFadeDuration = 0.5f;
        [SerializeField] private float pauseBetweenSteps = 0.8f;
        [SerializeField] private FloatingText floatingTextPrefab;
        [SerializeField] private string stepDoneText = "Bersih!";
        [SerializeField] private Color stepDoneColor = new(0.2f, 0.65f, 0.35f);
        [SerializeField] private ResultPopup finishPopup;

        private readonly List<PlaqueZone> zones = new();
        private int stepIndex = -1;
        private float stepStart;
        private bool finishingStep;

        public event Action Completed;

        private void Start()
        {
            if (steps.Length != TrainingSession.StepCount)
            {
                Debug.LogWarning($"[Brushing] {steps.Length} steps assigned, but TrainingSession expects {TrainingSession.StepCount}.");
            }

            StartStep(0);
        }

        // --- Steps ---
        private void StartStep(int index)
        {
            stepIndex = index;
            BrushStepData step = steps[index];

            mouthImage.sprite = step.mouthImage;
            instructionLabel.text = step.instruction;
            motionLabel.text = step.motionLabel;
            progressLabel.text = $"{index + 1}/{steps.Length}";
            hint.SetText(step.hint);

            SpawnZones(step);
            scrubber.Begin(step.motion, zones);
            stepStart = Time.time;
            finishingStep = false;
        }

        private void SpawnZones(BrushStepData step)
        {
            foreach (PlaqueZone zone in zones) Destroy(zone.gameObject);
            zones.Clear();

            Rect layer = zoneLayer.rect;
            foreach (Rect normalized in step.zones)
            {
                PlaqueZone zone = Instantiate(zonePrefab, zoneLayer);
                RectTransform rect = (RectTransform)zone.transform;
                rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = new Vector2(normalized.width * layer.width, normalized.height * layer.height);
                rect.anchoredPosition = new Vector2(
                    layer.xMin + normalized.center.x * layer.width,
                    layer.yMin + normalized.center.y * layer.height);
                zone.Build();
                zones.Add(zone);
            }
        }

        // --- Progress ---
        private void Update()
        {
            if (finishingStep || stepIndex < 0) return;
            if (Cleanliness() < cleanThreshold) return;

            StartCoroutine(FinishStep());
        }

        private float Cleanliness()
        {
            float total = 0f;
            foreach (PlaqueZone zone in zones) total += zone.Cleanliness;
            return zones.Count == 0 ? 1f : total / zones.Count;
        }

        private IEnumerator FinishStep()
        {
            finishingStep = true;
            scrubber.Stop();
            if (stepIndex < TrainingSession.StepCount)
            {
                TrainingSession.RecordStep(stepIndex + 1, Time.time - stepStart, steps[stepIndex].parTime);
            }

            AudioPlayer.PlaySound(SoundEffect.Correct);
            FloatingText text = Instantiate(floatingTextPrefab, zoneLayer);
            text.GetComponent<TMP_Text>().color = stepDoneColor;
            text.Show(stepDoneText);

            for (float t = 0f; t < finishFadeDuration; t += Time.deltaTime)
            {
                foreach (PlaqueZone zone in zones) zone.CleanAll(t / finishFadeDuration);
                yield return null;
            }
            foreach (PlaqueZone zone in zones) zone.CleanAll(1f);

            yield return new WaitForSeconds(pauseBetweenSteps);

            if (stepIndex + 1 < steps.Length)
            {
                StartStep(stepIndex + 1);
                yield break;
            }

            Completed?.Invoke();
            finishPopup.ShowCorrect(TrainingSession.Total);
        }
    }
}
