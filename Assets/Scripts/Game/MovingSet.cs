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

        private Vector3 trueEndPoint;
        private Vector3 currentStartPoint;
        private Vector3 currentEndPoint;
        private Vector3 currentDirection;

        protected override void Start()
        {
            base.Start();
            DetermineTrueEndPoint();
        }

        /// <summary>
        /// Track reached current endpoint.
        /// </summary>
        /// <returns></returns>
        private bool TrackReachedEndpoint()
        {
            return targetTrack.position == currentEndPoint;
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
        /// Make multiple passes before the player until either told to stop
        /// or after a certain number of passes.
        /// </summary>
        protected override void ExecuteMainSequence()
        {
            base.ExecuteMainSequence();

            if (TrackReachedEndpoint())
            {
                currentPass++;
                ChangeDirectionAndTarget();
            }

            if (loop || currentPass < totalPasses)
            {
                TranslateTrack(currentEndPoint, currentDirection, speed);
            }
            else
            {
                StopTargetSet();
            }
        }

        /// <summary>
        /// Move targets out of view toward current endpoint.
        /// </summary>
        protected override void ExecuteStopSequence()
        {
            base.ExecuteStopSequence();

            if (TrackReachedEndpoint())
            {
                RemoveTargets();
            }
            else
            {
                TranslateTrack(currentEndPoint, currentDirection, speed);
            }
        }

        /// <summary>
        /// Reset passes, direction, start/end points, and track position.
        /// </summary>
        protected override void ResetTargetSet()
        {
            base.ResetTargetSet();
            currentPass = 0;
            currentDirection = direction;
            currentStartPoint = targetRack.GetStartPoint();
            currentEndPoint = trueEndPoint;
            targetTrack.position = targetRack.GetStartPoint();
        }

        /// <summary>
        /// Change current target endpoint and direction.
        /// </summary>
        private void ChangeDirectionAndTarget()
        {
            if (!loop && currentPass >= totalPasses) return;

            currentDirection *= -1;
            Vector3 temp = currentEndPoint;
            currentEndPoint = currentStartPoint;
            currentStartPoint = temp;
        }
    }
}
