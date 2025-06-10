using ShootingGallery.Interfaces;
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
        [Tooltip("The number target types that will appear in the set in order left to right.")]
        [SerializeField]
        private TargetType[] setOrder;

        [SerializeField]
        private int setMultiplier;

        [SerializeField]
        private TargetPool targetPool;
        
        [SerializeField]
        protected Transform startPoint; 
        
        [SerializeField]
        protected Transform endPoint;

        [SerializeField]
        protected float distanceBetweenTargets = 5.0f;
        
        protected Vector3 direction;

        protected List<ShootingTarget> shootingTargets = new List<ShootingTarget>();

        protected int totalTargets = 0;
        protected int totalDecoys = 0;
        protected int targetsHit = 0;

        public UnityAction<int> onTargetHit;
        public UnityAction onTargetSetComplete;

        protected virtual void Start()
        {
            direction = (endPoint.position - startPoint.position).normalized;
            DetermineTypeCounts();
        }

        /// <summary>
        /// Get targets from target pool and spawn them.
        /// </summary>
        public virtual void InitiateTargetSet()
        {
            AssignTargets();
            SpawnTargets();
        }

        // Can Keep
        public bool IsTargetSetFree()
        {
            return shootingTargets.Count == 0;
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
        /// Allocate targets from target pool to a local list.
        /// </summary>
        private void AssignTargets()
        {
            foreach (TargetType type in setOrder)
            {
                if (type == TargetType.Normal)
                {
                    shootingTargets.Add(targetPool.AllocateTarget(this));
                }
                else if (type == TargetType.Decoy)
                {
                    shootingTargets.Add(targetPool.AllocateDecoy(this));
                }
            }
        }

        /// <summary>
        /// Spawn targets at incremental distances out of the player's view.
        /// </summary>
        private void SpawnTargets()
        {
            for (int i = 0; i < shootingTargets.Count; i++)
            {
                Vector3 spawnPoint = startPoint.position - direction * (i * distanceBetweenTargets);
                shootingTargets[i].transform.position = spawnPoint;
                shootingTargets[i].gameObject.SetActive(true);
            }
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
                targetPool.DeallocateDecoy(target);
            }

            shootingTargets.Clear();
            onTargetSetComplete?.Invoke();
        }

        /// <summary>
        /// Inform round set on points awarded for successfully hitting a target.
        /// </summary>
        /// <param name="points">Points from target</param>
        /// <param name="type">Type of target</param>
        public void OnTargetHit(int points, TargetType type)
        {
            if (type == TargetType.Normal)
            {
                targetsHit++;
                points = points * setMultiplier;
            }

            onTargetHit?.Invoke(points);
        }
    }

}