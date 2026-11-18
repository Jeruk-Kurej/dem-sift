using System.Collections.Generic;
using UnityEngine;

namespace DEMSIFT.Training
{
    public class MotionClassifier
    {
        private const float MaxTurnPerSample = 120f * Mathf.Deg2Rad;
        private const float MinStepDistance = 2f;
        private const float MinDuration = 0.25f;

        private readonly Queue<Vector2> positions = new();
        private readonly Queue<float> times = new();
        private readonly float window;
        private readonly float minSpeed;
        private Vector2 lastPosition;
        private float lastTime;

        public MotionClassifier(float windowSeconds, float minimumSpeed)
        {
            window = windowSeconds;
            minSpeed = minimumSpeed;
        }

        // --- Samples ---
        public void Add(Vector2 position, float time)
        {
            if (positions.Count > 0 && Vector2.Distance(lastPosition, position) < MinStepDistance) return;

            lastPosition = position;
            lastTime = time;
            positions.Enqueue(position);
            times.Enqueue(time);

            while (times.Count > 0 && time - times.Peek() > window)
            {
                positions.Dequeue();
                times.Dequeue();
            }
        }

        public void Clear()
        {
            positions.Clear();
            times.Clear();
        }

        // --- Scoring (0 = no match, 1 = clear match) ---
        public float Score(BrushMotion motion)
        {
            if (positions.Count < 4) return 0f;

            Features f = Measure();
            float duration = Mathf.Max(lastTime - times.Peek(), MinDuration);
            if (f.Path / duration < minSpeed) return 0f;

            return motion switch
            {
                BrushMotion.UpDown => AxisScore(f.TravelY, f.TravelX, f.ReversalsY),
                BrushMotion.BackForth => AxisScore(f.TravelX, f.TravelY, f.ReversalsX),
                BrushMotion.Circle => CircleScore(f),
                BrushMotion.Sweep => SweepScore(f),
                _ => 0f,
            };
        }

        private static float AxisScore(float along, float across, int reversals)
        {
            float dominance = along / Mathf.Max(along + across, 0.001f);
            return Mathf.Clamp01((dominance - 0.55f) / 0.2f) * Mathf.Clamp01(reversals / 2f);
        }

        private static float CircleScore(Features f)
        {
            float turns = Mathf.Abs(f.SignedTurn) / (2f * Mathf.PI);
            float consistency = Mathf.Abs(f.SignedTurn) / Mathf.Max(f.AbsoluteTurn, 0.001f);
            return Mathf.Clamp01(turns / 0.75f) * Mathf.Clamp01((consistency - 0.5f) / 0.3f);
        }

        private static float SweepScore(Features f)
        {
            float straightness = f.Net.magnitude / Mathf.Max(f.Path, 0.001f);
            return Mathf.Clamp01((straightness - 0.45f) / 0.25f);
        }

        // --- Features ---
        private struct Features
        {
            public float Path;
            public float TravelX;
            public float TravelY;
            public int ReversalsX;
            public int ReversalsY;
            public float SignedTurn;
            public float AbsoluteTurn;
            public Vector2 Net;
        }

        private Features Measure()
        {
            Features f = default;
            Vector2? previous = null;
            Vector2 previousDelta = Vector2.zero;
            Vector2 first = Vector2.zero;
            float signX = 0f;
            float signY = 0f;

            foreach (Vector2 position in positions)
            {
                if (previous == null)
                {
                    previous = first = position;
                    continue;
                }

                Vector2 delta = position - previous.Value;
                f.Path += delta.magnitude;
                f.TravelX += Mathf.Abs(delta.x);
                f.TravelY += Mathf.Abs(delta.y);

                CountReversal(delta.x, ref signX, ref f.ReversalsX);
                CountReversal(delta.y, ref signY, ref f.ReversalsY);

                if (previousDelta != Vector2.zero)
                {
                    float turn = Vector2.SignedAngle(previousDelta, delta) * Mathf.Deg2Rad;
                    if (Mathf.Abs(turn) < MaxTurnPerSample)
                    {
                        f.SignedTurn += turn;
                        f.AbsoluteTurn += Mathf.Abs(turn);
                    }
                }

                previousDelta = delta;
                previous = position;
            }

            f.Net = previous.Value - first;
            return f;
        }

        private static void CountReversal(float delta, ref float sign, ref int reversals)
        {
            if (Mathf.Abs(delta) < MinStepDistance) return;

            float current = Mathf.Sign(delta);
            if (sign != 0f && current != sign) reversals++;
            sign = current;
        }

    }
}
