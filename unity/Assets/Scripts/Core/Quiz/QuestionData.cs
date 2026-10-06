using UnityEngine;

namespace DEMSIFT.Quiz
{
    [CreateAssetMenu(fileName = "NewQuestion", menuName = "DEM-SIFT/Quiz Question")]
    public class QuestionData : ScriptableObject
    {
        [TextArea(2, 4)]
        public string questionText;

        [Header("Answer Options")]
        public string[] options;

        [Tooltip("Only for image answers. Leave empty for text answers; when filled it must match the options length.")]
        public Sprite[] optionImages;

        public int correctAnswerIndex;
    }
}
