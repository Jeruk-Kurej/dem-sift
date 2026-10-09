using System.Collections.Generic;
using UnityEngine;

namespace DEMSIFT.Puzzle
{
    [CreateAssetMenu(menuName = "DEM-SIFT/Puzzle Data", fileName = "PuzzleData")]
    public class PuzzleData : ScriptableObject
    {
        [System.Serializable]
        public class PuzzlePiece
        {
            public string pieceId;
            [Tooltip("Text shown on the piece. Leave empty to show the piece id.")]
            public string label;
            public Sprite pieceSprite;
            public string correctZoneId;
        }

        public string questionText;
        public List<PuzzlePiece> pieces;
    }
}
