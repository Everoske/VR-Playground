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

    [Header("Max Magazines Associated with Refill Box")]
    [SerializeField]
    private int maxMagazines = 10;

    [SerializeField]
    private XRMagazine magazinePrefab;

    private XRMagazine[] magazinePool;


    private void InitiateMagazinePoll()
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
        // Ensure magazine exists
        // Move magazine to spawn point
        // Set game object to active
        // Set magazine count to max
    }

    private void DespawnMagazine(int index)
    {
        // Ensure magazine exists
        // Set game object to inactive
        // Move magazine to hidden area (0.0f, -1000.0f, 0.0f)
    }

}
