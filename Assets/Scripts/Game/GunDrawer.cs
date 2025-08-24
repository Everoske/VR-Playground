using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace ShootingGallery.Game
{
    /// <summary>
    /// Handles spawning guns in at the start of a game set and respawning guns
    /// that go out of bounds
    /// </summary>
    public class GunDrawer : MonoBehaviour
    {
        [SerializeField]
        private Transform[] largeGunSpawns;

        [SerializeField]
        private Transform[] smallGunSpawns;
        
        private Animator animator;

        private const string animatorOpenRef = "Open";

        private List<GameObject> instancedWeapons = new List<GameObject>();

        public UnityAction onDrawerClose;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }


        public void SpawnGuns(GameObject smallGunPrefab, GameObject largeGunPrefab)
        {
            if (smallGunPrefab != null)
            {
                SpawnSmallGun(smallGunPrefab);
            }

            if (largeGunPrefab != null)
            {
                SpawnLargeGun(largeGunPrefab);
            }

            OpenDrawer();
        }

        public void DespawnGuns()
        {
            // Assign a new tag to all guns so they cannot be used by the player

            // Move guns back into gun drawer

            // Close gun drawer
            CloseDrawer();
        }

        private void OpenDrawer()
        {
            animator.SetBool(animatorOpenRef, true);
        }

        private void CloseDrawer()
        {
            animator.SetBool(animatorOpenRef, false);
        }

        private void SpawnSmallGun(GameObject smallGunPrefab)
        {
            SpawnGun(smallGunPrefab, smallGunSpawns);
        }

        private void SpawnLargeGun(GameObject largeGunPrefab)
        {
            SpawnGun(largeGunPrefab, largeGunSpawns);
        }

        private void SpawnGun(GameObject gunPrefab, Transform[] possibleSpawns)
        {
            int randomIndex = Random.Range(0, possibleSpawns.Length);
            GameObject instancedGun = Instantiate(gunPrefab, possibleSpawns[randomIndex].position, possibleSpawns[randomIndex].rotation);
            instancedWeapons.Add(instancedGun);
        }
    }
}
