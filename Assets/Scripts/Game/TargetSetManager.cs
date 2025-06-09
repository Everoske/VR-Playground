using ShootingGallery.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ShootingGallery.Game
{
    public class TargetSetManager : MonoBehaviour, ITargetHitNotify
    {
        [SerializeField]
        private TargetSet[] targetSets;

        [SerializeField]
        private ShootingTarget stationaryTargetPrefab;
        [SerializeField]
        private ShootingTarget stationaryDecoyPrefab;
        [SerializeField]
        private ShootingTarget movingTargetPrefab;
        [SerializeField]
        private ShootingTarget movingDecoyPrefab;

        [SerializeField]
        private Transform startPoint;
        [SerializeField]
        private Transform endPoint;

        [Tooltip("Initial Size for Object Pools")]
        [SerializeField]
        private int initialPoolSize = 10;

        [SerializeField]
        private Transform targetParent;

        private ShootingTarget[] movingTargetPool;
        private ShootingTarget[] movingDecoyPool;
        private ShootingTarget[] stationaryTargetPool;
        private ShootingTarget[] stationaryDecoyPool;

        private int activeSetIndex;
        private int targetsThisRound;

        public UnityAction<int> onTargetHit;
        public UnityAction onRoundComplete; // Needs examination

        private void Start()
        {
            CreatePools();
        }

        private void CreatePools()
        {
            if (targetParent == null)
            {
                targetParent = transform;
            }

            CreatePool(out movingTargetPool, TargetType.Normal, ref movingTargetPrefab);
            CreatePool(out movingDecoyPool, TargetType.Decoy, ref movingDecoyPrefab);
            CreatePool(out stationaryTargetPool, TargetType.Normal, ref stationaryTargetPrefab);
            CreatePool(out stationaryDecoyPool, TargetType.Decoy, ref stationaryDecoyPrefab);
        }

        private void CreatePool(out ShootingTarget[] pool, TargetType type, ref ShootingTarget targetPrefab)
        {
            pool = new ShootingTarget[initialPoolSize];

            for (int i = 0; i < initialPoolSize; i++)
            {
                pool[i] = Instantiate(
                    targetPrefab,
                    new Vector3(0.0f, -1000.0f, 0.0f),
                    startPoint.rotation, targetParent
                    );
                pool[i].TargetHitNotify = this;
                pool[i].TargetType = type;
                pool[i].gameObject.SetActive(false);
            }
        }

        // Can Keep
        private void ExpandPool(ref ShootingTarget[] pool, TargetType type, ref ShootingTarget targetPrefab)
        {
            ShootingTarget[] temp = new ShootingTarget[pool.Length + initialPoolSize];

            for (int i = 0; i < pool.Length; i++)
            {
                temp[i] = pool[i];
                pool[i] = null;
            }

            for (int i = pool.Length - 1; i < temp.Length; i++)
            {
                temp[i] = Instantiate(
                    targetPrefab,
                    new Vector3(0.0f, -1000.0f, 0.0f),
                    startPoint.rotation, targetParent
                    );
                temp[i].TargetHitNotify = this;
                temp[i].TargetType = type;
                temp[i].gameObject.SetActive(false);
            }

            pool = new ShootingTarget[pool.Length + initialPoolSize];

            for (int i = 0; i < pool.Length; i++)
            {
                pool[i] = temp[i];
                temp[i] = null;
            }
        }

        public void OnTargetHit(int points, TargetType type)
        {
            if (type == TargetType.Normal)
            {
                targetsThisRound--;
                // points = points * targetSets[activeSetIndex].GetSetMultiplier();
            }

            onTargetHit?.Invoke(points);
        }

        private void EnsurePoolCapacity(ref ShootingTarget[] targetPool, int targetsThisRound, ref ShootingTarget targetPrefab)
        {
            if (targetsThisRound > targetPool.Length)
            {
                ExpandPool(ref targetPool, TargetType.Normal, ref targetPrefab);
            }
        }

        private void ActivateActiveSet()
        {
            SetType setType = targetSets[activeSetIndex].GetSetType();

            if (setType == SetType.Stationary)
            {
                EnsurePoolCapacity(ref stationaryTargetPool, targetSets[activeSetIndex].GetTargetCount(), ref stationaryTargetPrefab);
                EnsurePoolCapacity(ref stationaryDecoyPool, targetSets[activeSetIndex].GetDecoyCount(), ref stationaryDecoyPrefab);
                AssignTargetsToSet(ref stationaryTargetPool, ref stationaryDecoyPool);
            }
            else
            {
                EnsurePoolCapacity(ref movingTargetPool, targetSets[activeSetIndex].GetTargetCount(), ref movingTargetPrefab);
                EnsurePoolCapacity(ref movingDecoyPool, targetSets[activeSetIndex].GetDecoyCount(), ref movingDecoyPrefab);
                AssignTargetsToSet(ref movingTargetPool, ref movingDecoyPool);
            }

            targetSets[activeSetIndex].StartSet();
        }

        // Can Keep
        private void AssignTargetsToSet(ref ShootingTarget[] targetPool, ref ShootingTarget[] decoyPool)
        {
            int targetIndex = 0;
            int decoyIndex = 0;

            for (int i = 0; i < targetSets[activeSetIndex].GetSetOrder().Length; i++)
            {
                if (targetSets[activeSetIndex].GetSetOrder()[i] == TargetType.Normal)
                {
                    AssignTarget(ref targetPool, targetIndex);
                    targetIndex++;
                }
                else
                {
                    AssignTarget(ref decoyPool, decoyIndex);
                    decoyIndex++;
                }
            }
        }

        private void AssignTarget(ref ShootingTarget[] targetPool, int index)
        {
            targetSets[activeSetIndex].AssignTarget(targetPool[index]);
        }
    }
}
