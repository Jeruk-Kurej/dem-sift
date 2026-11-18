using UnityEngine;

namespace DEMSIFT.Training
{
    public enum BrushMotion
    {
        UpDown,
        BackForth,
        Circle,
        Sweep,
    }

    [CreateAssetMenu(fileName = "NewBrushStep", menuName = "DEM-SIFT/Brush Step")]
    public class BrushStepData : ScriptableObject
    {
        [TextArea(2, 4)]
        public string instruction;
        public string motionLabel;
        public string hint;

        public Sprite mouthImage;

        [Tooltip("Normalized rects (0-1, origin bottom-left) on the mouth image.")]
        public Rect[] zones;

        public BrushMotion motion;

        [Tooltip("Expected seconds for this step; used later for the score.")]
        public float parTime = 15f;
    }
}
