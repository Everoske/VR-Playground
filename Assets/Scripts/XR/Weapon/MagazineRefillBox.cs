using System.Collections;
using UnityEngine;

namespace ShootingGallery.XR.Weapon
{
    public class MagazineRefillBox : MonoBehaviour
    {
        [SerializeField]
        private Transform spawnPoint;

        [SerializeField]
        private Transform magazineParent;

        [Tooltip("Max Magazines Associated with Refill Box")]
        [SerializeField]
        private int maxMagazines = 10;

        [SerializeField]
        private XRMagazine magazinePrefab;

        private XRMagazine[] magazinePool;

        private int spawnedMagazines = 0;
        private bool canAllocate = true;

        private void Start()
        {
            InitiateMagazinePool();
        }

        private void Update()
        {
            if (canAllocate)
            {
                StartCoroutine(AllocateMagazines());
            }
        }

        private IEnumerator AllocateMagazines()
        {
            canAllocate = false;
            float waitTime = 0.1f;
            yield return new WaitForSeconds(waitTime);

            SpawnSingleInactive();
            canAllocate = true;
            if (spawnedMagazines >= maxMagazines)
            {
                canAllocate = false;
            }
        }

        private void SpawnSingleInactive()
        {
            if (spawnedMagazines >= maxMagazines) return;

            for (int i = 0; i < magazinePool.Length; i++)
            {
                if (magazinePool[i] != null && !magazinePool[i].gameObject.activeInHierarchy)
                {
                    SpawnMagazine(i);
                    break;
                }
            }
        }

        private void InitiateMagazinePool()
        {
            magazinePool = new XRMagazine[maxMagazines];

            for (int i = 0; i < magazinePool.Length; i++)
            {
                magazinePool[i] = Instantiate(
                    magazinePrefab,
                    new Vector3(-0.0f, -1000.0f, 0.0f),
                    Quaternion.identity,
                    magazineParent != null ? magazineParent : transform
                );

                magazinePool[i].gameObject.SetActive(false);
            }
        }

        private void SpawnMagazine(int index)
        {
            magazinePool[index].transform.position = spawnPoint.position;
            magazinePool[index].transform.rotation = spawnPoint.rotation;
            magazinePool[index].gameObject.SetActive(true);
            magazinePool[index].SetAmmoToMax();
            spawnedMagazines++;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.parent.gameObject.TryGetComponent<XRMagazine>(out XRMagazine magazine))
            {
                if (magazine.IsUsed() || magazine.HasMaxAmmo()) return;

                magazine.gameObject.SetActive(false);
                magazine.SetAmmoToMax();
                magazine.transform.position = spawnPoint.position;
                magazine.transform.rotation = spawnPoint.rotation;
                magazine.gameObject.SetActive(true);
            }
        }
    }
}
