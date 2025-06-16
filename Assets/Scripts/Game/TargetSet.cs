using ShootingGallery.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ShootingGallery.Game
{
    /// <summary>
    /// Represents a single set of similar targets for the player to hit.
    /// </summary>
    public class TargetSet : MonoBehaviour, ITargetHitNotify
    {
        [SerializeField]
        private TargetPool targetPool;

        [Tooltip("The number target types that will appear in the set in order left to right.")]
        [SerializeField]
        protected TargetType[] setOrder;

        [SerializeField]
        protected Transform targetTrack;

        [SerializeField]
        private int targetPoints = 25;

        [SerializeField]
        private int decoyPoints = -25;

        [SerializeField]
        protected TargetRack targetRack;

        [SerializeField]
        protected float distanceBetweenTargets = 5.0f;
        
        protected Vector3 direction;

        protected ShootingTarget[] shootingTargets;

        protected int totalTargets = 0;
        protected int totalDecoys = 0;
        protected int targetsHit = 0;

        public UnityAction onTargetHit;
        public UnityAction<bool> onTargetSetComplete;

        public bool IsDecoyOnly()
        {
            return totalTargets <= 0 && totalDecoys > 0;
        }

        protected virtual void Awake()
        {
            DetermineTypeCounts();
        }

        protected virtual void Start()
        {
            direction = targetRack.GetRackDirection();
            shootingTargets = new ShootingTarget[setOrder.Length];
            targetTrack.position = targetRack.GetStartPoint();
        }

        private void CheckAllTargetsHit()
        {
            if (targetsHit >= totalTargets)
            {
                StopTargetSet();
            }
        }

        /// <summary>
        /// Get targets from target pool and spawn them.
        /// </summary>
        public virtual void InitiateTargetSet()
        {
            targetsHit = 0;
            if (shootingTargets.Length == 0)
            {
                ReleaseTargetSet();
                return;
            }

            SpawnTargets();
        }

        // Can Keep
        public bool IsTargetSetFree()
        {
            return shootingTargets.Length == 0;
        }


        /// <summary>
        /// Determines how many normal targets and decoys are in this target set.
        /// </summary>
        private void DetermineTypeCounts()
        {
            foreach (TargetType type in setOrder)
            {
                if (type == TargetType.Normal)
                {
                    totalTargets++;
                }
                else if (type == TargetType.Decoy)
                {
                    totalDecoys++;
                }
            }
        }

        /// <summary>
        /// Allocate targets from target pool to a local list. This should be
        /// done only once per GalleryRound.
        /// </summary>
        public void AssignTargets()
        {
            for (int i = 0; i < setOrder.Length; i++)
            {
                TargetType type = setOrder[i]; 
                if (type == TargetType.Normal)
                {
                    shootingTargets[i] = targetPool.AllocateTarget(this);
                }
                else if (type == TargetType.Decoy)
                {
                    shootingTargets[i] = targetPool.AllocateDecoy(this);
                }

                shootingTargets[i].transform.parent = targetTrack;
            }
        }

        public void UnassignTargets()
        {
            RemoveTargets();
        }

        public int GetTotalTargetSetScore()
        {
            return totalTargets * targetPoints;
        }

        /// <summary>
        /// Spawn targets at incremental distances out of the player's view.
        /// </summary>
        private void SpawnTargets()
        {
            for (int i = 0; i < shootingTargets.Length; i++)
            {
                Vector3 spawnPoint = targetRack.GetStartPoint() - direction * (i * distanceBetweenTargets);
                shootingTargets[i].transform.position = spawnPoint;
                shootingTargets[i].gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Clears shooting targets and informs round set that stop sequence has been completed.
        /// </summary>
        private void ReleaseTargetSet()
        {
            for (int i = 0; i < shootingTargets.Length; i++)
            {
                shootingTargets[i] = null;
            }
            
            onTargetSetComplete?.Invoke(IsDecoyOnly());
        }

        /// <summary>
        /// Initiates the stop sequence
        /// </summary>
        public virtual void StopTargetSet()
        {

        }

        /// <summary>
        /// Returns targets to Target Pool. Should be called after targets are
        /// out of the player's view
        /// </summary>
        protected virtual void RemoveTargets()
        {
            foreach (ShootingTarget target in shootingTargets)
            {
                targetPool.DeallocateShootingTarget(target);
            }

            ReleaseTargetSet();
        }

        /// <summary>
        /// Moves target track toward the target position at the given speed and direction.
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <param name="currentDirection"></param>
        /// <param name="speed"></param>
        protected void TranslateTrack(Vector3 targetPosition, Vector3 currentDirection, float speed)
        {
            speed = speed * Time.deltaTime;
            if (speed >= (targetPosition - targetTrack.position).magnitude)
            {
                speed = (targetPosition - targetTrack.position).magnitude;
            }

            targetTrack.transform.Translate(currentDirection * speed);
        }

        /// <summary>
        /// Inform round set on points awarded for successfully hitting a target.
        /// </summary>
        /// <param name="points">Points from target</param>
        /// <param name="type">Type of target</param>
        public void OnTargetHit(TargetType type)
        {
            int points;
            if (type == TargetType.Normal)
            {
                targetsHit++;
                points = targetPoints;
                AccuracyLocator.GetAccuracyTracker().IncrementTargetsHit();
                CheckAllTargetsHit();
            }
            else
            {
                points = decoyPoints;
            }

            ScoreLocator.GetScoreTracker().AddToScore(points);
            onTargetHit?.Invoke();
        }
    }
}