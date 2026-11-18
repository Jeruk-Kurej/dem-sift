using System.Collections.Generic;

namespace DEMSIFT.Data
{
    public abstract class ResultFormat
    {
        public abstract IEnumerable<string> ScoreHeaders();

        public abstract IEnumerable<string> CurrentScores();

        public abstract int CurrentTotal { get; }
    }
}
