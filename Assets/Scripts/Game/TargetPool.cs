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
                //pool[i].TargetHitNotify = this;
                pool[i].TargetType = type;
                pool[i].gameObject.SetActive(false);
            }
        }
    }
}
