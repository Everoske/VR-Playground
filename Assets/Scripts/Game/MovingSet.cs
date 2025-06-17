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

        [Tooltip("The track will continue to move until told to stop.")]
        [SerializeField]
        private bool loop = false;

        [Tooltip("Determines how many times the track will pass by the player. Ignored if loop set to true.")]
        [SerializeField]
        private int totalPasses = 2;

        private int currentPass;
        private bool canMove = false;

        private Vector3 trueEndPoint;
        private Vector3 currentStartPoint;
        private Vector3 currentEndPoint;
        private Vector3 currentDirection;

        protected override void Start()
        {
            base.Start();
            DetermineTrueEndPoint();
        }

        private void MoveTrack()
        {
            if (TrackReachedEndpoint())
            {
                currentPass++;
                currentDirection *= -1;
                Vector3 temp = currentEndPoint;
                currentEndPoint = currentStartPoint;
                currentStartPoint = temp;
                
                if (currentPass >= totalPasses)
                {
                    RemoveTargets();
                    return;
                }
            }

            TranslateTrack(currentEndPoint, currentDirection, speed);
        }

        private bool TrackReachedEndpoint()
        {
            return targetTrack.position == currentEndPoint;
        }

        private bool CanMove()
        {
            return canMove && currentPass <= totalPasses;
        }

        /// <summary>
        /// Sets the total track length based on the spawn location of the last target.
        /// </summary>
        private void DetermineTrueEndPoint()
        {
            Vector3 lastSpawn = targetRack.GetStartPoint() -
                direction * (distanceBetweenTargets * (shootingTargets.Length - 1));
            float spawnOffset = Vector3.Distance(targetRack.GetStartPoint(), lastSpawn);
            trueEndPoint = targetRack.GetEndPoint() + spawnOffset * direction;
        }

        /// <summary>
        /// Moving set should be able to process movement upon initiation.
        /// </summary>
        public override void InitiateTargetSet()
        {
            base.InitiateTargetSet();
            if (shootingTargets.Length == 0) return;

            canMove = true;
            currentPass = 0;
            currentDirection = direction;
            currentStartPoint = targetRack.GetStartPoint();
            currentEndPoint = trueEndPoint;
        }

        /// <summary>
        /// Targets should continue moving until out of view and then despawn.
        /// </summary>
        public override void StopTargetSet()
        {
            base.StopTargetSet();
            if (!IsSetActive()) return;

            currentPass = totalPasses;
        }

        protected override void ExecuteMainSequence()
        {
            base.ExecuteMainSequence();
            if (CanMove())
            {
                MoveTrack();
            }
        }

        /// <summary>
        /// Moving set should not move after targets have been removed.
        /// </summary>
        protected override void RemoveTargets()
        {
            base.RemoveTargets();
            canMove = false;
            targetTrack.position = targetRack.GetStartPoint();
        }
    }
}
