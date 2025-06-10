using System.Net;
using UnityEngine;

namespace ShootingGallery.Game
{
    /// <summary>
    /// A moving implementation of target set. Targets move between two points at a certain speed
    /// during a set amount of loops or until time runs out in the round set. 
    /// </summary>
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

        protected override void Update()
        {
            base.Update();
            if (canMove && currentLoop <= totalLoops)
            {
                MoveTargets();
            }
        }

        /// <summary>
        /// Moves targets between end points.
        /// </summary>
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

        /// <summary>
        /// Determines if lead target has traveled the entire length of one track.
        /// </summary>
        /// <returns>True if lead target has reached an endpoint of the track.</returns>
        private bool LeadTargetReachedEndpoint()
        {
            if (shootingTargets.Count <= 0) return false;
            ShootingTarget leadTarget = shootingTargets[leadIndex];
            if (leadTarget == null) return false;

            return Vector3.Distance(leadTarget.transform.position, currentStartPoint) >= totalTrackLength;
        }

        /// <summary>
        /// Sets the total track length based on the spawn location of the last target.
        /// </summary>
        private void SetTotalTrackLength()
        {
            Vector3 lastSpawn = startPoint.position -
                direction * (distanceBetweenTargets * (shootingTargets.Count - 1));
            float spawnOffset = Vector3.Distance(startPoint.position, lastSpawn);
            totalTrackLength = trackLength + spawnOffset;
        }

        /// <summary>
        /// Moving set should be able to process movement upon initiation.
        /// </summary>
        public override void InitiateTargetSet()
        {
            base.InitiateTargetSet();
            if (shootingTargets.Count == 0) return;

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

        /// <summary>
        /// Moving set should not move after targets have been removed.
        /// </summary>
        protected override void RemoveTargets()
        {
            base.RemoveTargets();
            canMove = false;
        }
    }
}
