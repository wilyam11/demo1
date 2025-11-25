using System;


public enum Judgment
{
    Perfect,
    Good,
    Miss,
    None
}

public class JudgementData
{
    public float PerfectInterval { get; private set; }
    public float GoodInterval { get; private set; }

    public const float PERFECT = 3f / 4f;
    public const float GOOD = 7f / 4f;

    public JudgementData(float bpm)
    {
        float _spb_4 = 60f / bpm / 4f;

        PerfectInterval = PERFECT * _spb_4;
        GoodInterval = GOOD * _spb_4;
    }

    public Judgment Judge(float delay)
    {
        float _delay = Math.Abs(delay);

        if (_delay <= PerfectInterval)
        {
            return Judgment.Perfect;
        }
        else if (_delay <= GoodInterval)
        {
            return Judgment.Good;
        }
        else
        {
            if (delay < 0)
            {
                return Judgment.Miss;
            }

            return Judgment.None;
        }
    }
}
