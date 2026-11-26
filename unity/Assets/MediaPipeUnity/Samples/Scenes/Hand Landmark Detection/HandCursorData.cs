using UnityEngine;

namespace DEMSIFT.HandTracking
{
    public static class HandCursorData
    {
        private static readonly object syncRoot = new object();
        private static bool hasHand;
        private static float cursorX;
        private static float cursorY;
        private static float pinchRatio = 1f;
        private static bool hasPalm;
        private static Vector2 palmCenter;
        private static Vector2 palmAxis;

        public static void PublishPalm(Vector2 center, Vector2 axis)
        {
            lock (syncRoot)
            {
                hasPalm = true;
                palmCenter = center;
                palmAxis = axis;
            }
        }

        public static bool TryGetPalm(out Vector2 center, out Vector2 axis)
        {
            lock (syncRoot)
            {
                center = palmCenter;
                axis = palmAxis;
                return hasHand && hasPalm;
            }
        }

        public static void Publish(bool handFound, float x, float y, float ratio)
        {
            lock (syncRoot)
            {
                hasHand = handFound;
                if (!handFound) hasPalm = false;
                cursorX = x;
                cursorY = y;
                pinchRatio = ratio;
            }
        }

        public static bool TryGet(out float x, out float y, out float ratio)
        {
            lock (syncRoot)
            {
                x = cursorX;
                y = cursorY;
                ratio = pinchRatio;
                return hasHand;
            }
        }

        // --- Play mode without domain reload ---
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Clear()
        {
            Publish(false, 0f, 0f, 1f);
        }
    }
}
