using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

public class ScoreData
{
    private Dictionary<Judgment, int> scoreTable = new Dictionary<Judgment, int>()
    {
        { Judgment.Perfect, 0 },
        { Judgment.Good, 0 },
        { Judgment.Miss, 0 }
    };

    public ScoreData()
    {

    }

    public  void Reset()
    {
        scoreTable.ToList().ForEach(kv => scoreTable[kv.Key] = 0);
    }

    public void Add(Judgment judgment, int count = 1)
    {
        if (scoreTable.ContainsKey(judgment))
        {
            scoreTable[judgment] += count;
        }
    }

    public int Get(Judgment judgment)
    {
        if (scoreTable.ContainsKey(judgment))
        {
            return scoreTable[judgment];
        }

        return -1;
    }
}