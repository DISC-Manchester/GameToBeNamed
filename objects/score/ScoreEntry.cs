using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquareSmash.objects.score
{
    [Serializable]
    public class ScoreEntry
    {
        public string UserName { get; set; } = "";
        public int Score { get; set; } = 0;
        public ScoreEntry() { }

        public ScoreEntry(string userName, int score)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            Score = score;
        }

        public override string? ToString()
        {
            return $"ScoreEntry(UserName:{UserName},\n\t\tScore:{Score})";
        }
    }
}
