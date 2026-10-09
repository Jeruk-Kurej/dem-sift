using UnityEngine;

namespace DEMSIFT.UI
{
    public class SwayMotion : MonoBehaviour
    {
        [SerializeField] private float angle = 2f;
        [SerializeField] private float period = 4f;

        private float startAngle;
        private float phase;

        private void Awake()
        {
            startAngle = transform.localEulerAngles.z;
            phase = Random.value * Mathf.PI * 2f;
        }

        private void Update()
        {
            float wave = Mathf.Sin(Time.time * Mathf.PI * 2f / period + phase);
            transform.localRotation = Quaternion.Euler(0f, 0f, startAngle + angle * wave);
        }
    }
}
