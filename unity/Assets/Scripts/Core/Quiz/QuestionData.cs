using UnityEngine;

namespace DemSift.Core.Quiz
{
    // A single quiz question as reusable data, not code.
    // Create one via: right-click in Project > Create > DemSift > Quiz Question
    [CreateAssetMenu(fileName = "NewQuestion", menuName = "DemSift/Quiz Question")]
    public class QuestionData : ScriptableObject
    {
        [TextArea(2, 4)]
        public string questionText;

        [Header("Answer Options")]
        public string[] options;

        // Optional: only fill this in if this question's answers are images
        // (e.g. Pengetahuan 2 - pick the correct tooth photo). Leave empty
        // for text-only questions. If filled, must match "options" length.
        public Sprite[] optionImages;

        public int correctAnswerIndex;
    }
}
