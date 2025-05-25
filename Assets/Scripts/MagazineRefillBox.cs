using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 'Spawn' loaded magazines periodically, track spawned magazines,
// 'despawn' unloaded magazines
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
    private float spawnTime = 5.0f;

    [SerializeField]
    private XRMagazine magazinePrefab;

    private XRMagazine[] magazinePool;
    private Stack<int> emptyMagIndices;

    private int spawnedMagazines = 0;
    private bool canAllocate = false;
    private bool shouldSpawnInitial = true;

    private void Start()
    {
        InitiateMagazinePool();
        emptyMagIndices = new Stack<int>();
    }

    private void Update()
    {
        if (shouldSpawnInitial)
        {
            StartCoroutine(SpawnInitial());
        }

        if (canAllocate)
        {
            StartCoroutine(AllocateMagazines());
        }
    }

    private IEnumerator SpawnInitial()
    {
        yield return new WaitForSeconds(0.1f);
        SpawnSingleInactive();

        if (spawnedMagazines <= 10)
        {
            canAllocate = true;
            shouldSpawnInitial = false;
        }
    }

    private IEnumerator AllocateMagazines()
    {
        canAllocate = false;
        yield return new WaitForSeconds(spawnTime);

        DespawnSingleEmpty();
        SpawnSingleInactive();
        canAllocate = true;
    }

    private void DespawnSingleEmpty()
    {
        if (spawnedMagazines <= 0) return;

        DespawnNextEmpty();

        for (int i = 0; i < magazinePool.Length; i++)
        {
            if (IsEmptyAndUnused(i) && !emptyMagIndices.Contains(i))
            {
                emptyMagIndices.Push(i);
            }
        }
    }

    private void DespawnNextEmpty()
    {
        if (emptyMagIndices.Count == 0) return;

        int magToDespawn = emptyMagIndices.Pop();
        bool despawnSuccess = false;

        while (!despawnSuccess && emptyMagIndices.Count > 0)
        {
            if (!magazinePool[magToDespawn].IsUsed())
            {
                DespawnMagazine(magToDespawn);
                despawnSuccess = true;
            }
            else
            {
                magToDespawn = emptyMagIndices.Pop();
            }
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
        magazinePool[index].gameObject.SetActive(true);
        magazinePool[index].SetAmmoToMax();
        spawnedMagazines++;
    }

    private void DespawnMagazine(int index)
    {
        magazinePool[index].gameObject.SetActive(false);
        magazinePool[index].transform.position = new Vector3(0.0f, -1000.0f, 0.0f);
        spawnedMagazines--;
    }

    private bool IsEmptyAndUnused(int index)
    {
        return magazinePool[index] != null &&
            magazinePool[index].CurrentAmmo <= 0 &&
            !magazinePool[index].IsUsed();
    }
}
