using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [SerializeField]
    private List<TargetRack> targetRacks;

    [SerializeField]
    private List<RoundData> rounds;

    [SerializeField]
    private ScoreTracker scoreTracker;

    [SerializeField]
    private RoundUI roundUI;

    [SerializeField]
    private float timeBetweenRounds = 5.0f;

    private int numberOfRounds;
    private int stride;

    private int currentRound = 0;
    private int racksCompletedThisRound = 0;
    private float roundTimer = 0.0f;
    private bool timerActive = false;

    private void Start()
    {
        stride = targetRacks.Count;
        if (stride > 0)
        {
            numberOfRounds = rounds.Count / stride;
            Debug.Log(numberOfRounds);
        }

        scoreTracker.CalculateHighestScore(rounds);

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

    private void Update()
    {
        ProcessTimer();
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
        roundUI.SetCurrentRoundText(currentRound);

        for (int i = 0; i < targetRacks.Count; i++)
        {
            int roundIndex = i + (currentRound - 1) * stride;
            if (roundIndex < rounds.Count)
            {
                targetRacks[i].InitiateRound(rounds[roundIndex]);
            }
            else
            {
                EndGame();
            }
        }
    }

    private void StartRoundTimer()
    {
        roundTimer = timeBetweenRounds;
        timerActive = true;
        roundUI.ActivateTimerUI();
    }

    private void ProcessTimer()
    {
        if (!timerActive) return;

        roundTimer -= Time.deltaTime;

        if (roundTimer <= 0.0f)
        {
            roundUI.SetTimerText(0, 0);
            timerActive = false;
            roundUI.DeactivateTimerUI();
            StartCurrentRound();
        }
        else
        {
            int minutes = (int)roundTimer / 60;
            int seconds = (int)roundTimer % 60;
            roundUI.SetTimerText(minutes, seconds);
        }

    }

    private void EndGame()
    {
        currentRound = 0;
        racksCompletedThisRound = 0;
    }

    private void AddToScore(int value)
    {
        scoreTracker.CurrentScore = scoreTracker.CurrentScore + value;
        roundUI.SetScoreText(scoreTracker.CurrentScore);
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
                StartRoundTimer();
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
