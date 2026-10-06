using System.Collections.Generic;
using UnityEngine;

namespace DEMSIFT.Puzzle
{
    public class TrayScatter : MonoBehaviour
    {
        [SerializeField] private Vector2 spread = new(450f, 90f);
        [SerializeField] private int candidatesPerPiece = 12;

        public void Scatter()
        {
            var placed = new List<Vector2>();

            foreach (RectTransform piece in transform)
            {
                piece.anchorMin = piece.anchorMax = piece.pivot = new Vector2(0.5f, 0.5f);

                Vector2 spot = PickSpot(placed);
                piece.anchoredPosition = spot;
                piece.SetSiblingIndex(Random.Range(0, transform.childCount));
                placed.Add(spot);
            }
        }

        // --- Best-candidate sampling ---
        private Vector2 PickSpot(List<Vector2> placed)
        {
            Vector2 best = RandomPoint();
            float bestDist = MinDistance(best, placed);

            for (int i = 1; i < candidatesPerPiece; i++)
            {
                Vector2 candidate = RandomPoint();
                float dist = MinDistance(candidate, placed);
                if (dist > bestDist) { best = candidate; bestDist = dist; }
            }
            return best;
        }

        private Vector2 RandomPoint() =>
            new(Random.Range(-spread.x, spread.x), Random.Range(-spread.y, spread.y));

        private static float MinDistance(Vector2 point, List<Vector2> others)
        {
            float min = float.MaxValue;
            foreach (var other in others) min = Mathf.Min(min, Vector2.Distance(point, other));
            return min;
        }
    }
}
