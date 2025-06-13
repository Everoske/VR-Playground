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
        [Tooltip("The number target types that will appear in the set in order left to right.")]
        [SerializeField]
        protected TargetType[] setOrder;

        [SerializeField]
        protected Transform targetParent;

        [SerializeField]
        private int setMultiplier;
        
        [SerializeField]
        protected Transform startPoint; 
        
        [SerializeField]
        protected Transform endPoint;

        [SerializeField]
        protected float distanceBetweenTargets = 5.0f;
        
        protected Vector3 direction;

        protected ShootingTarget[] shootingTargets;

        protected SetType setType;
        protected int totalTargets = 0;
        protected int totalDecoys = 0;
        protected int targetsHit = 0;

        public UnityAction<int> onTargetHit;
        public UnityAction onTargetSetComplete;

        protected virtual void Start()
        {
            direction = (endPoint.position - startPoint.position).normalized;
            shootingTargets = new ShootingTarget[setOrder.Length];
            DetermineTypeCounts();
        }

        protected virtual void Update()
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
            // Automatically close an empty target set
            targetsHit = 0;
            if (shootingTargets.Length == 0)
            {
                CloseTargetSet();
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
        public void AssignTargets(ref TargetPool pool)
        {
            for (int i = 0; i < setOrder.Length; i++)
            {
                TargetType type = setOrder[i]; 
                if (type == TargetType.Normal)
                {
                    shootingTargets[i] = pool.AllocateTarget(this, setType);
                }
                else if (type == TargetType.Decoy)
                {
                    shootingTargets[i] = pool.AllocateDecoy(this, setType);
                }

                shootingTargets[i].transform.parent = targetParent;
            }
        }

        /// <summary>
        /// Spawn targets at incremental distances out of the player's view.
        /// </summary>
        private void SpawnTargets()
        {
            for (int i = 0; i < shootingTargets.Length; i++)
            {
                Vector3 spawnPoint = startPoint.position - direction * (i * distanceBetweenTargets);
                shootingTargets[i].transform.position = spawnPoint;
                shootingTargets[i].gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Clears shooting targets and informs round set that stop sequence has been completed.
        /// </summary>
        private void CloseTargetSet()
        {
            for (int i = 0; i < shootingTargets.Length; i++)
            {
                shootingTargets[i] = null;
            }
            
            onTargetSetComplete?.Invoke();
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
                target.ResetTarget();
                target.gameObject.SetActive(false);
                target.transform.position = new Vector3(0.0f, -1000.0f, 0.0f);
            }

            CloseTargetSet();
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