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

        private int currentLoop;
        private bool canMove = false;

        private Vector3 trueEndPoint;
        private Vector3 currentStartPoint;
        private Vector3 currentEndPoint;
        private Vector3 currentDirection;

        private void Awake()
        {
            setType = SetType.Moving;
        }
        protected override void Start()
        {
            base.Start();
            DetermineTrueEndPoint();
        }

        protected override void Update()
        {
            base.Update();
            if (canMove && currentLoop <= totalLoops)
            {
                MoveTrack();
            }
        }

        private void MoveTrack()
        {
            if (TrackReachedEndpoint())
            {
                currentLoop++;
                currentDirection *= -1;
                Vector3 temp = currentEndPoint;
                currentEndPoint = currentStartPoint;
                currentStartPoint = temp;
                
                if (currentLoop >= totalLoops)
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

        /// <summary>
        /// Sets the total track length based on the spawn location of the last target.
        /// </summary>
        private void DetermineTrueEndPoint()
        {
            Vector3 lastSpawn = startPoint.position -
                direction * (distanceBetweenTargets * (shootingTargets.Length - 1));
            float spawnOffset = Vector3.Distance(startPoint.position, lastSpawn);
            trueEndPoint = endPoint.position + spawnOffset * direction;
        }

        /// <summary>
        /// Moving set should be able to process movement upon initiation.
        /// </summary>
        public override void InitiateTargetSet()
        {
            base.InitiateTargetSet();
            if (shootingTargets.Length == 0) return;

            canMove = true;
            currentLoop = 0;
            currentDirection = direction;
            currentStartPoint = startPoint.position;
            currentEndPoint = trueEndPoint;
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
