using ShootingGallery.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ShootingGallery.Game
{
    public class TargetSet : MonoBehaviour, ITargetHitNotify
    {
        [Tooltip("The number target types that will appear in the set in order left to right")]
        [SerializeField]
        private TargetType[] setOrder;

        [SerializeField]
        private int setMultiplier;

        [SerializeField]
        private TargetPool targetPool;

        protected Transform startPoint; 
        protected Transform endPoint; 
        protected float distanceBetweenTargets;
        protected Vector3 direction;

        protected List<ShootingTarget> shootingTargets = new List<ShootingTarget>();

        protected int totalTargets = 0;
        protected int totalDecoys = 0;
        protected int targetsHit = 0;

        public UnityAction<int> onTargetHit;
        public UnityAction onTargetSetComplete;

        public int GetTargetCount() => totalTargets;
        public int GetDecoyCount() => totalDecoys;
        public int GetSetMultiplier() => setMultiplier;

        public TargetType[] GetSetOrder() => setOrder;

        private void DetermineSetCounts()
        {
            foreach (ShootingTarget target in shootingTargets)
            {
                if (target.TargetType == TargetType.Normal)
                {
                    totalTargets++;
                }
                else if (target.TargetType == TargetType.Decoy)
                {
                    totalDecoys++;
                }
            }
        }

        protected virtual void Start()
        {
            direction = (startPoint.position - endPoint.position).normalized;
            DetermineSetCounts();
        }

        public void InitiateTargetSet()
        {
            AssignTargets();
            SpawnTargets();
        }

        // Can Keep
        public bool IsTargetRackFree()
        {
            return shootingTargets.Count == 0;
        }

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

        private void SpawnTargets()
        {
            for (int i = 0; i < shootingTargets.Count; i++)
            {
                Vector3 spawnPoint = startPoint.position - direction * (i * distanceBetweenTargets);
                shootingTargets[i].transform.position = spawnPoint;
                shootingTargets[i].gameObject.SetActive(true);
            }
        }

        public virtual void StopTargetSet()
        {

        }

        protected virtual void RemoveTargets()
        {
            foreach (ShootingTarget target in shootingTargets)
            {
                targetPool.DeallocateDecoy(target);
            }

            shootingTargets.Clear();
            onTargetSetComplete?.Invoke();
        }

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