using UnityEngine;

namespace DEMSIFT.Training
{
    public abstract class BrushInput : MonoBehaviour
    {
        public abstract bool IsBrushing { get; }
        public abstract Vector2 ScreenPosition { get; }
    }
}
