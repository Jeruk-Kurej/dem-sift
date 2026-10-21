using UnityEngine;

namespace DEMSIFT.HandTracking
{
    public class OneEuroFilter
    {
        private const float DerivativeCutoff = 1f;

        private Vector2 value;
        private Vector2 velocity;
        private bool hasValue;

        public void Reset()
        {
            hasValue = false;
        }

        public Vector2 Filter(Vector2 sample, float deltaTime, float minCutoff, float beta)
        {
            if (!hasValue)
            {
                value = sample;
                velocity = Vector2.zero;
                hasValue = true;
                return value;
            }

            if (deltaTime <= 0f) return value;

            Vector2 rawVelocity = (sample - value) / deltaTime;
            velocity = Vector2.Lerp(velocity, rawVelocity, Alpha(DerivativeCutoff, deltaTime));

            float cutoff = minCutoff + beta * velocity.magnitude;
            value = Vector2.Lerp(value, sample, Alpha(cutoff, deltaTime));
            return value;
        }

        private static float Alpha(float cutoff, float deltaTime)
        {
            float timeConstant = 1f / (2f * Mathf.PI * cutoff);
            return 1f / (1f + timeConstant / deltaTime);
        }
    }
}
