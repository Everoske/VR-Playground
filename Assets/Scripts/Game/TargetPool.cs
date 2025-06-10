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

        private int firstFreeTarget = 0;
        private int firstFreeDecoy = 0;

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

        // Could be optimized by including a index in the ShootingTarget class
        private int GetFirstActiveIndex(ref ShootingTarget[] pool)
        {
            for (int i = 0; i < pool.Length; i++)
            {
                if (!pool[i].IsTargetInUse)
                {
                    return i;
                }
            }

            return -1;
        }

        public ShootingTarget AllocateTarget(ITargetHitNotify hitNotify)
        {
            if (allocatedTargets > targetPool.Length)
            {
                ExpandPool(ref targetPool, TargetType.Normal, ref targetPrefab);
                firstFreeTarget = targetPool.Length - 1;
            }

            allocatedTargets++;
            int index = firstFreeTarget;
            targetPool[index].IsTargetInUse = true;
            targetPool[index].TargetHitNotify = hitNotify;

            firstFreeTarget = GetFirstActiveIndex(ref targetPool);

            return targetPool[index];
        }

        public void DeallocateTarget(ShootingTarget shootingTarget)
        {
            // Ensure shootingTarget belongs to object
            if (shootingTarget.transform.parent != transform) return;

            firstFreeTarget = GetFirstActiveIndex(ref targetPool);

            shootingTarget.ResetTarget();
            shootingTarget.transform.position = new Vector3(0.0f, -1000.0f, 0.0f);
            shootingTarget.gameObject.SetActive(false);

            allocatedTargets--;
        }

        public ShootingTarget AllocateDecoy(ITargetHitNotify hitNotify)
        {
            if (allocatedDecoys > decoyPool.Length)
            {
                ExpandPool(ref decoyPool, TargetType.Decoy, ref decoyPrefab);
                firstFreeDecoy = decoyPool.Length - 1;
            }

            allocatedDecoys++;
            int index = firstFreeDecoy;
            decoyPool[index].IsTargetInUse = true;
            decoyPool[index].TargetHitNotify = hitNotify;

            firstFreeDecoy = GetFirstActiveIndex(ref decoyPool);

            return decoyPool[index];
        }

        public void DeallocateDecoy(ShootingTarget shootingDecoy)
        {
            // Ensure shootingTarget belongs to object
            if (shootingDecoy.transform.parent != transform) return;

            firstFreeDecoy = GetFirstActiveIndex(ref decoyPool);

            shootingDecoy.ResetTarget();
            shootingDecoy.transform.position = new Vector3(0.0f, -1000.0f, 0.0f);
            shootingDecoy.gameObject.SetActive(false);

            allocatedDecoys--;
        }
    }
}
