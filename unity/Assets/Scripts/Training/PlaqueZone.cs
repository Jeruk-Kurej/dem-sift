using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DEMSIFT.Training
{
    [RequireComponent(typeof(RectTransform))]
    public class PlaqueZone : MonoBehaviour
    {
        [SerializeField] private Image spotPrefab;
        [SerializeField] private float spotsPerArea = 0.00012f;
        [SerializeField] private int minSpots = 2;
        [SerializeField] private Vector2 spotSizeRange = new(46f, 70f);

        private readonly List<Image> spots = new();
        private readonly List<float> dirt = new();
        private RectTransform rectTransform;

        public float Cleanliness
        {
            get
            {
                float total = 0f;
                foreach (float amount in dirt) total += amount;
                return dirt.Count == 0 ? 1f : 1f - total / dirt.Count;
            }
        }

        // --- Setup ---
        public void Build()
        {
            rectTransform = (RectTransform)transform;
            Rect rect = rectTransform.rect;
            int count = Mathf.Max(minSpots, Mathf.RoundToInt(rect.width * rect.height * spotsPerArea));

            for (int i = 0; i < count; i++)
            {
                Image spot = Instantiate(spotPrefab, transform);
                RectTransform spotRect = (RectTransform)spot.transform;
                float size = Random.Range(spotSizeRange.x, spotSizeRange.y);
                spotRect.sizeDelta = new Vector2(size, size);
                spotRect.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
                spotRect.anchoredPosition = SpreadPosition(rect, i, count, size * 0.5f);
                spots.Add(spot);
                dirt.Add(1f);
            }
        }

        private static Vector2 SpreadPosition(Rect rect, int index, int count, float margin)
        {
            bool wide = rect.width >= rect.height;
            float slot = (index + 0.5f) / count;
            float alongMin = (wide ? rect.xMin : rect.yMin) + margin;
            float alongMax = (wide ? rect.xMax : rect.yMax) - margin;
            float acrossMin = (wide ? rect.yMin : rect.xMin) + margin;
            float acrossMax = (wide ? rect.yMax : rect.xMax) - margin;

            float along = Mathf.Lerp(alongMin, alongMax, slot) + Random.Range(-margin, margin) * 0.4f;
            float across = Random.Range(acrossMin, Mathf.Max(acrossMin, acrossMax));
            return wide ? new Vector2(along, across) : new Vector2(across, along);
        }

        // --- Brushing ---
        public bool Contains(Vector2 localPointInParent)
        {
            Vector2 local = localPointInParent - (Vector2)rectTransform.localPosition;
            return rectTransform.rect.Contains(local);
        }

        public void Scrub(Vector2 localPointInParent, float radius, float amount)
        {
            Vector2 local = localPointInParent - (Vector2)rectTransform.localPosition;

            for (int i = 0; i < spots.Count; i++)
            {
                if (dirt[i] <= 0f) continue;

                float distance = Vector2.Distance(local, ((RectTransform)spots[i].transform).anchoredPosition);
                if (distance > radius) continue;

                SetDirt(i, dirt[i] - amount);
            }
        }

        public void CleanAll(float progress)
        {
            for (int i = 0; i < spots.Count; i++)
            {
                SetDirt(i, Mathf.Min(dirt[i], 1f - progress));
            }
        }

        private void SetDirt(int index, float amount)
        {
            dirt[index] = Mathf.Clamp01(amount);
            Color color = spots[index].color;
            color.a = dirt[index];
            spots[index].color = color;
        }
    }
}
