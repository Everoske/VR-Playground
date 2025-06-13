using ShootingGallery.Interfaces;
using UnityEngine;

namespace ShootingGallery.Game
{
    // Contains object pools for targets and decoys
    // Uses Target Prefabs
    // Create a subproject to test this code in C#
    public class TargetPool : MonoBehaviour
    {
        [SerializeField]
        private ShootingTarget stationaryTargetPrefab;
        [SerializeField]
        private ShootingTarget stationaryDecoyPrefab;
        [SerializeField]
        private ShootingTarget movingTargetPrefab;
        [SerializeField]
        private ShootingTarget movingDecoyPrefab;

        [Tooltip("Initial Size for Object Pools")]
        [SerializeField]
        private int initialPoolSize = 25;

        private ShootingTarget[] stationaryTargetPool; // rename to stationaryTargetPool
        private ShootingTarget[] stationaryDecoyPool; // rename to stationaryDecoyPool
        private ShootingTarget[] movingTargetPool;
        private ShootingTarget[] movingDecoyPool;

        private int allocatedStationaryTargets = 0;
        private int allocatedStationaryDecoys = 0;
        private int allocatedMovingTargets = 0;
        private int allocatedMovingDecoys = 0;

        private void Start()
        {
            CreatePools();
        }

        private void CreatePools()
        {
            CreatePool(out stationaryTargetPool, TargetType.Normal, ref stationaryTargetPrefab);
            CreatePool(out stationaryDecoyPool, TargetType.Decoy, ref stationaryDecoyPrefab);
            CreatePool(out movingTargetPool, TargetType.Normal, ref movingTargetPrefab);
            CreatePool(out movingDecoyPool, TargetType.Decoy, ref  movingDecoyPrefab);
        }

        private void CreatePool(out ShootingTarget[] pool, TargetType type, ref ShootingTarget targetPrefab)
        {
            pool = new ShootingTarget[initialPoolSize];

            for (int i = 0; i < initialPoolSize; i++)
            {
                pool[i] = Instantiate(
                    targetPrefab,
                    new Vector3(0.0f, -1000.0f, 0.0f),
                    transform.rotation, transform
                    );
                pool[i].TargetType = type;
                pool[i].gameObject.SetActive(false);
            }
        }

        private void ExpandPool(ref ShootingTarget[] pool, TargetType type, ref ShootingTarget poolPrefab)
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
                    poolPrefab,
                    new Vector3(0.0f, -1000.0f, 0.0f),
                    transform.rotation, transform
                    );
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

        public ShootingTarget AllocateTarget(ITargetHitNotify hitNotify, SetType setType)
        {
            if (setType == SetType.Moving)
            {
                return AllocateShootingTarget(hitNotify, ref movingTargetPool, 
                    ref movingTargetPrefab, TargetType.Normal, ref allocatedMovingTargets);
            }

            return AllocateShootingTarget(hitNotify, ref stationaryTargetPool, 
                ref stationaryTargetPrefab, TargetType.Normal, ref allocatedStationaryTargets);
        }

        public void DeallocateTarget(ShootingTarget shootingTarget)
        {
            // Ensure shootingTarget belongs to object
            if (shootingTarget.transform.parent != transform) return;

            shootingTarget.ResetTarget();
            shootingTarget.transform.position = new Vector3(0.0f, -1000.0f, 0.0f);
            shootingTarget.gameObject.SetActive(false);
            allocatedStationaryTargets--;
        }

        public ShootingTarget AllocateDecoy(ITargetHitNotify hitNotify, SetType setType)
        {
            if (setType == SetType.Moving)
            {
                return AllocateShootingTarget(hitNotify, ref movingDecoyPool,
                    ref movingDecoyPrefab, TargetType.Normal, ref allocatedMovingDecoys);
            }

            return AllocateShootingTarget(hitNotify, ref stationaryDecoyPool,
                ref stationaryDecoyPrefab, TargetType.Normal, ref allocatedStationaryDecoys);
        }

        public void DeallocateDecoy(ShootingTarget shootingDecoy)
        {
            // Ensure shootingTarget belongs to object
            if (shootingDecoy.transform.parent != transform) return;

            shootingDecoy.ResetTarget();
            shootingDecoy.transform.position = new Vector3(0.0f, -1000.0f, 0.0f);
            shootingDecoy.gameObject.SetActive(false);
            allocatedStationaryDecoys--;
        }

        public void FreePools()
        {
            allocatedMovingDecoys = 0;
            allocatedMovingTargets = 0;
            allocatedStationaryDecoys = 0;
            allocatedStationaryTargets = 0;
        }

        private ShootingTarget AllocateShootingTarget(ITargetHitNotify hitNotify, ref ShootingTarget[] pool,
            ref ShootingTarget targetPrefab, TargetType type, ref int numberAllocated)
        {
            if (numberAllocated > pool.Length)
            {
                ExpandPool(ref pool, type, ref targetPrefab);
            }

            numberAllocated++;
            int index = numberAllocated - 1;
            pool[index].TargetHitNotify = hitNotify;
            return pool[index];
        }
    }
}
