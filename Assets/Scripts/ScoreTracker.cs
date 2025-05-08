using System.Collections.Generic;
using UnityEngine;

public class ScoreTracker : MonoBehaviour
{
    private int highestPossibleScore;
    private int currentScore;

    public int CurrentScore
    {
        get => currentScore;
        set
        {
            currentScore = Mathf.Max(value, 0);
        }
    }

    public void CalculateHighestScore(List<RoundData> rounds)
    {
        foreach (RoundData round in rounds)
        {
            foreach(TargetType target in round.targets)
            {
                if (target == TargetType.Normal)
                {
                    highestPossibleScore += 25 * round.roundMultiplier;
                }
            }
        }

        Debug.Log(highestPossibleScore);
    }

    public void ResetScore()
    {
        currentScore = 0;
    }
}
