using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// CHANGES TO IMPLEMENT:
// - Utilize Magazine Pool
// - On Start
//   - Spawn all magazines on a timer
// - Have a Space for Player to Recycle Magazines
//   - When Magazine enters recycle space, spawn a new full magazine
// - Coroutine only needed for initial magazines
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

    private int spawnedMagazines = 0;
    private bool canAllocate = true;
    private bool initialSpawned = false;

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
        float waitTime = !initialSpawned ? 0.1f : spawnTime;
        yield return new WaitForSeconds(waitTime);

        DespawnSingleEmpty();
        SpawnSingleInactive();
        canAllocate = true;

        if (!initialSpawned && spawnedMagazines >= maxMagazines)
        {
            initialSpawned = false;
        }
    }

    private void DespawnSingleEmpty()
    {
        if (spawnedMagazines <= 0) return;

        for (int i = 0; i < magazinePool.Length; i++)
        {
            if (IsEmptyUnusedAndActive(i))
            {
                DespawnMagazine(i);
                break;
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

    private bool IsEmptyUnusedAndActive(int index)
    {
        return magazinePool[index] != null &&
            magazinePool[index].gameObject.activeInHierarchy &&
            magazinePool[index].CurrentAmmo <= 0 &&
            !magazinePool[index].IsUsed();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<XRMagazine>(out XRMagazine magazine))
        {
            magazine.gameObject.SetActive(false);
            magazine.SetAmmoToMax();
            magazine.transform.position = spawnPoint.position;
            magazine.transform.rotation = spawnPoint.rotation;
            magazine.gameObject.SetActive(true);
        }
    }
}
