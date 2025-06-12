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

        private int allocatedStationTargets = 0;
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

        public ShootingTarget AllocateTarget(ITargetHitNotify hitNotify)
        {
            if (allocatedStationTargets > stationaryTargetPool.Length)
            {
                ExpandPool(ref stationaryTargetPool, TargetType.Normal, ref stationaryTargetPrefab);
            }

            allocatedStationTargets++;
            int index = allocatedStationTargets - 1;
            stationaryTargetPool[index].TargetHitNotify = hitNotify;

            return stationaryTargetPool[index];
        }

        public void DeallocateTarget(ShootingTarget shootingTarget)
        {
            // Ensure shootingTarget belongs to object
            if (shootingTarget.transform.parent != transform) return;

            shootingTarget.ResetTarget();
            shootingTarget.transform.position = new Vector3(0.0f, -1000.0f, 0.0f);
            shootingTarget.gameObject.SetActive(false);
            allocatedStationTargets--;
        }

        public ShootingTarget AllocateDecoy(ITargetHitNotify hitNotify)
        {
            if (allocatedStationaryDecoys > stationaryDecoyPool.Length)
            {
                ExpandPool(ref stationaryDecoyPool, TargetType.Decoy, ref stationaryDecoyPrefab);
            }

            allocatedStationaryDecoys++;
            int index = allocatedStationaryDecoys - 1;
            stationaryDecoyPool[index].TargetHitNotify = hitNotify;

            return stationaryDecoyPool[index];
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
    }
}
