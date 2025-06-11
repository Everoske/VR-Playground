using ShootingGallery.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShootingGallery.Game
{
    public class RoundManager : MonoBehaviour
    {
        [SerializeField]
        private List<TargetSet> targetRacks; // Obsolete

        [SerializeField]
        private List<RoundData> rounds; // Obsolete

        [SerializeField]
        private ScoreTracker scoreTracker; // Move to GameSet

        [SerializeField]
        private RoundUI roundUI; // Move to GameSet

        [SerializeField]
        private float timeBetweenRounds = 5.0f; // Obsolete

        private int numberOfRounds; // Obsolete
        private int stride; // Obsolete

        private int currentRound = 0; // Obsolete
        private int racksCompletedThisRound = 0; // Obsolete
        private float roundTimer = 0.0f; // Obsolete
        private bool timerActive = false; // Obsolete

        private void Start()
        {
            stride = targetRacks.Count;
            if (stride > 0)
            {
                numberOfRounds = rounds.Count / stride;
            }

            scoreTracker.CalculateHighestScore(rounds);
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

        public void StartGame()
        {
            if (IsGameActive() || !TargetRacksFree()) return;
            StartRounds();
        }

        public void StopGame()
        {
            EndGame();
        }


        private void StartRounds()
        {
            if (numberOfRounds <= 0) return;
            currentRound = 1;
            scoreTracker.CurrentScore = 0;
            roundUI.SetScoreText(scoreTracker.CurrentScore);
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
                    //targetRacks[i].InitiateRound(rounds[roundIndex]);
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
            roundTimer = 0.0f;
            timerActive = false;
            roundUI.DeactivateTimerUI();

            foreach (TargetSet targetRack in targetRacks)
            {
                targetRack.StopTargetSet();
            }
        }

        private void AddToScore(int value)
        {
            if (!IsGameActive()) return;
            scoreTracker.CurrentScore = scoreTracker.CurrentScore + value;
            roundUI.SetScoreText(scoreTracker.CurrentScore);
        }

        private void RackRoundCompleted()
        {
            if (!IsGameActive()) return;
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

        private bool TargetRacksFree()
        {
            foreach (TargetSet targetRack in targetRacks)
            {
                // Target racks with targets still visible to player
                if (!targetRack.IsTargetSetFree()) return false;
            }

            return true;
        }

        private bool IsGameActive()
        {
            return currentRound > 0;
        }

        private void RegisterTargetRacks()
        {
            foreach (TargetSet targetRack in targetRacks)
            {
                //targetRack.onTargetHit += AddToScore;
                //targetRack.onRoundComplete += RackRoundCompleted;
            }
        }

        private void DeregisterTargetRacks()
        {
            foreach (TargetSet targetRack in targetRacks)
            {
                //targetRack.onTargetHit -= AddToScore;
                //targetRack.onRoundComplete -= RackRoundCompleted;
            }
        }
    }
}
