using System.Net;
using UnityEngine;

namespace ShootingGallery.Game
{
    // Spawn Targets Outside View
    // Move Between Two Points at a Certain Speed
    // Move Outside View on Condition
    // Despawn Targets Outside View
    public class MovingSet : TargetSet
    {
        private float speed;
        private int totalLoops;
        private float trackLength;
        private int currentLoop;
        private int leadIndex; // Under Question
        private bool canMove = false;
        private float totalTrackLength;

        private Vector3 currentStartPoint;
        private Vector3 currentEndPoint;
        private Vector3 currentDirection;

        protected override void Start()
        {
            base.Start();
            setType = SetType.Moving;
            trackLength = Vector3.Distance(startPoint.position, endPoint.position);
        }

        private void Update()
        {
            if (canMove && currentLoop <= totalLoops)
            {
                MoveTargets();
            }

            if (targetHits >= totalTargets)
            {
                currentLoop = totalLoops;
            }
        }

        private void MoveTargets()
        {
            if (LeadTargetReachedEndpoint())
            {
                currentLoop++;
                currentDirection *= -1;
                Vector3 temp = currentEndPoint;
                currentEndPoint = currentStartPoint;
                currentStartPoint = temp;
                leadIndex = leadIndex > 0 ? 0 : shootingTargets.Count - 1;

                if (currentLoop >= totalLoops)
                {
                    DespawnTargets();
                    return;
                }
            }

            for (int i = 0; i < shootingTargets.Count; i++)
            {
                shootingTargets[i].transform.Translate(currentDirection * speed * Time.deltaTime);
            }
        }

        private bool LeadTargetReachedEndpoint()
        {
            if (shootingTargets.Count <= 0) return false;
            ShootingTarget leadTarget = shootingTargets[leadIndex];
            if (leadTarget == null) return false;

            return Vector3.Distance(leadTarget.transform.position, currentStartPoint) >= totalTrackLength;
        }

        private void SetTotalTrackLength()
        {
            Vector3 lastSpawn = startPoint.position -
                direction * (distanceBetweenTargets * (shootingTargets.Count - 1));
            float spawnOffset = Vector3.Distance(startPoint.position, lastSpawn);
            totalTrackLength = trackLength + spawnOffset;
        }

        public override void TerminateRound()
        {
            base.TerminateRound();
            currentLoop = totalLoops;
        }

        protected override void DespawnTargets()
        {
            base.DespawnTargets();
            canMove = false;
        }


    }
}
