using UnityEngine;
using UnityEngine.UI;

namespace DEMSIFT.UI
{
    [RequireComponent(typeof(Graphic))]
    public class AlphaFlicker : MonoBehaviour
    {
        [SerializeField] private float minAlpha = 0.4f;
        [SerializeField] private float maxAlpha = 1f;
        [SerializeField] private float speed = 1f;

        private Graphic graphic;
        private float seed;

        private void Awake()
        {
            graphic = GetComponent<Graphic>();
            seed = Random.value * 100f;
        }

        private void Update()
        {
            float noise = Mathf.PerlinNoise(seed, Time.time * speed);
            Color color = graphic.color;
            color.a = Mathf.Lerp(minAlpha, maxAlpha, noise);
            graphic.color = color;
        }
    }
}
