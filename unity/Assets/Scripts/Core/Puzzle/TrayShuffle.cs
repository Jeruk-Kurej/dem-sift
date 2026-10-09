using UnityEngine;

namespace DEMSIFT.Puzzle
{
    public class TrayShuffle : TrayArranger
    {
        public override void Arrange()
        {
            for (int last = transform.childCount - 1; last > 0; last--)
            {
                transform.GetChild(Random.Range(0, last + 1)).SetSiblingIndex(last);
            }
        }
    }
}
