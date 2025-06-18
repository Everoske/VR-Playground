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
        private ShootingTarget targetPrefab;
        [SerializeField]
        private ShootingTarget decoyPrefab;

        [Tooltip("Initial Size for Object Pools")]
        [SerializeField]
        private int initialPoolSize = 25;

        private ShootingTarget[] targetPool; 
        private ShootingTarget[] decoyPool; 

        private int allocatedTargets = 0;
        private int allocatedDecoys = 0;

        private void Start()
        {
            CreatePools();
        }

        private void CreatePools()
        {
            CreatePool(out targetPool, TargetType.Normal, ref targetPrefab);
            CreatePool(out decoyPool, TargetType.Decoy, ref decoyPrefab);
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
            return AllocateShootingTarget(hitNotify, ref targetPool, 
                ref targetPrefab, TargetType.Normal, ref allocatedTargets);
        }

        public ShootingTarget AllocateDecoy(ITargetHitNotify hitNotify)
        {
            return AllocateShootingTarget(hitNotify, ref decoyPool,
                ref decoyPrefab, TargetType.Decoy, ref allocatedDecoys);
        }

        public void DeallocateShootingTarget(ShootingTarget shootingTarget)
        {
            shootingTarget.transform.parent = transform;
            shootingTarget.transform.position = new Vector3(0.0f, -1000.0f, 0.0f);
            shootingTarget.ResetTarget();
            shootingTarget.gameObject.SetActive(false);

            DecrementNumberAllocated(shootingTarget.TargetType);
        }

        private ShootingTarget AllocateShootingTarget(ITargetHitNotify hitNotify, ref ShootingTarget[] pool,
            ref ShootingTarget targetPrefab, TargetType type, ref int numberAllocated)
        {
            if (numberAllocated >= pool.Length)
            {
                ExpandPool(ref pool, type, ref targetPrefab);
            }

            numberAllocated++;
            int index = numberAllocated - 1;
            pool[index].TargetHitNotify = hitNotify;
            return pool[index];
        }

        private void DecrementNumberAllocated(TargetType type)
        {
            switch (type)
            {
                case TargetType.Normal:
                    allocatedTargets--;
                    break;
                case TargetType.Decoy:
                    allocatedDecoys--;
                    break;
            }
        }
    }
}
