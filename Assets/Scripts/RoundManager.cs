using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [SerializeField]
    private List<TargetRack> targetRacks;

    [SerializeField]
    private List<RoundData> rounds;

    private int numberOfRounds;
    private int stride;

    private int currentRound = 0;
    private int racksCompletedThisRound = 0;

    private void Start()
    {
        stride = targetRacks.Count;
        if (stride > 0)
        {
            numberOfRounds = rounds.Count / stride;
            Debug.Log(numberOfRounds);
        }
        StartCoroutine(DebugStartRounds());
    }

    private void OnEnable()
    {
        RegisterTargetRacks();
    }

    private void OnDisable()
    {
        DeregisterTargetRacks();    
    }

    private IEnumerator DebugStartRounds()
    {
        yield return new WaitForSeconds(5.0f);
        StartRounds();
    }

    private void StartRounds()
    {
        if (numberOfRounds <= 0) return;
        currentRound = 1;
        StartCurrentRound();
    }

    private void StartCurrentRound()
    {
        racksCompletedThisRound = 0;

        for (int i = 0; i < targetRacks.Count; i++)
        {
            int roundIndex = i + (currentRound - 1) * stride;
            if (roundIndex < rounds.Count)
            {
                targetRacks[i].InitiateRound(rounds[roundIndex]);
            }
            else
            {
                Debug.Log($"Invalid Round Index: {roundIndex}");
            }
        }
    }

    private void EndGame()
    {
        currentRound = 0;
        racksCompletedThisRound = 0;
        Debug.Log("Game Ended");
    }

    private void AddToScore(int value)
    {
        
    }

    private void RackRoundCompleted()
    {
        if (currentRound > numberOfRounds) return;

        racksCompletedThisRound++;
        if (racksCompletedThisRound >= targetRacks.Count)
        {
            currentRound++;
            if (currentRound > numberOfRounds)
            {
                EndGame();
            }
            else
            {
                StartCurrentRound();
            }  
        }
    }

    private void RegisterTargetRacks()
    {
        foreach (TargetRack targetRack in targetRacks)
        {
            targetRack.onTargetHit += AddToScore;
            targetRack.onRoundComplete += RackRoundCompleted;
        }
    }

    private void DeregisterTargetRacks()
    {
        foreach (TargetRack targetRack in targetRacks)
        {
            targetRack.onTargetHit -= AddToScore;
            targetRack.onRoundComplete -= RackRoundCompleted;
        }
    }
}
