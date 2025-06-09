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

        protected Transform startPoint; 
        protected Transform endPoint; 
        protected float distanceBetweenTargets;
        protected Vector3 direction;

        protected List<ShootingTarget> shootingTargets = new List<ShootingTarget>();

        protected SetType setType;
        protected int totalTargets;
        protected int totalDecoys;
        protected int targetHits;

        public UnityAction<int> onTargetHit;
        public UnityAction onTargetSetComplete;

        public SetType GetSetType() => setType;
        public int GetTargetCount() => totalTargets;
        public int GetDecoyCount() => totalDecoys;
        public int GetSetMultiplier() => setMultiplier;

        public TargetType[] GetSetOrder() => setOrder;

        protected virtual void Start()
        {
            direction = (startPoint.position - endPoint.position).normalized;
        }

        public void AssignTarget(ShootingTarget target)
        {
            shootingTargets.Add(target);
        }

        public void InitiateTargetSet()
        {
            SpawnTargets();
        }

        // Can Keep
        public bool IsTargetRackFree()
        {
            return shootingTargets.Count == 0;
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

        public virtual void TerminateRound()
        {

        }

        protected virtual void DespawnTargets()
        {
            foreach (ShootingTarget target in shootingTargets)
            {
                target.ResetTarget();
                target.gameObject.SetActive(false);
                target.transform.position = new Vector3(0.0f, -1000.0f, 0.0f);
            }

            shootingTargets.Clear();
            //onRoundComplete?.Invoke(); // Move to TargetSetManager
        }

        // Can Keep
        public void OnTargetHit(int points, TargetType type)
        {
            if (type == TargetType.Normal)
            {
                //targetsThisRound--;
                // points = points * targetSets[activeSetIndex].GetSetMultiplier();
            }

            onTargetHit?.Invoke(points);
        }
    }

}