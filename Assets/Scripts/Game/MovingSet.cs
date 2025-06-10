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
        [SerializeField]
        private float speed = 2.0f;

        [SerializeField]
        private int totalLoops = 2;

        private float trackLength;
        private int currentLoop;
        private int leadIndex; 
        private bool canMove = false;
        private float totalTrackLength;

        private Vector3 currentStartPoint;
        private Vector3 currentEndPoint;
        private Vector3 currentDirection;

        protected override void Start()
        {
            base.Start();
            trackLength = Vector3.Distance(startPoint.position, endPoint.position);
            SetTotalTrackLength();
        }

        private void Update()
        {
            if (canMove && currentLoop <= totalLoops)
            {
                MoveTargets();
            }

            if (targetsHit >= totalTargets)
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
                    RemoveTargets();
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

        public override void InitiateTargetSet()
        {
            base.InitiateTargetSet();
            canMove = true;
            currentDirection = direction;
        }

        /// <summary>
        /// Targets should continue moving until out of view and then despawn.
        /// </summary>
        public override void StopTargetSet()
        {
            base.StopTargetSet();
            currentLoop = totalLoops;
        }

        protected override void RemoveTargets()
        {
            base.RemoveTargets();
            canMove = false;
        }
    }
}
