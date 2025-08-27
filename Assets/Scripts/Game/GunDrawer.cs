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
        private Transform drawerSlide;
        [SerializeField]
        private Transform closedPosition;
        [SerializeField]
        private Transform openPosition;
        [SerializeField]
        private float slideTransitionSpeed = 1.0f;

        [SerializeField]
        private Transform[] largeGunSpawns;
        [SerializeField]
        private Transform[] smallGunSpawns;

        // Translation controls
        private Vector3 slideTargetPosition;
        private Vector3 slideDirection;
        private bool isTranslating = false;

        private List<GameObject> instancedWeapons = new List<GameObject>();

        public UnityAction onDrawerClose;

        private void Start()
        {
            drawerSlide.position = closedPosition.position;
        }

        private void Update()
        {
            if (isTranslating)
            {
                TranslateSlide();
                CheckTranslationComplete();
            }
        }

        /// <summary>
        /// Spawn guns for a game set and open gun drawer.
        /// </summary>
        /// <param name="smallGunPrefab"></param>
        /// <param name="largeGunPrefab"></param>
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

        /// <summary>
        /// Return active weapons to gun drawer, make them no longer interactable, 
        /// and begin closing gun drawer.
        /// </summary>
        public void InitiateRemoveActiveWeapons()
        {
            // Assign a new tag to all guns so they cannot be used by the player

            // Move guns back into gun drawer

            // Close gun drawer
            CloseDrawer();
        }

        /// <summary>
        /// Check if drawer is in process of closing.
        /// </summary>
        /// <returns></returns>
        public bool IsDrawerClosing()
        {
            return isTranslating && slideTargetPosition == closedPosition.position;
        }

        /// <summary>
        /// Begin moving drawer slide to the open position.
        /// </summary>
        private void OpenDrawer()
        {
            isTranslating = true;
            slideTargetPosition = openPosition.position;
            slideDirection = Vector3.Normalize(closedPosition.position - openPosition.position);
        }

        /// <summary>
        /// Begin moving drawer slide to the closed position.
        /// </summary>
        private void CloseDrawer()
        {
            isTranslating = true;
            slideTargetPosition = closedPosition.position;
            slideDirection = Vector3.Normalize(openPosition.position - closedPosition.position);
        }

        /// <summary>
        /// Spawn small gun at one of its potential spawn points.
        /// </summary>
        /// <param name="smallGunPrefab"></param>
        private void SpawnSmallGun(GameObject smallGunPrefab)
        {
            SpawnGun(smallGunPrefab, smallGunSpawns);
        }

        /// <summary>
        /// Spawn large gun at one of its potential spawn points.
        /// </summary>
        /// <param name="largeGunPrefab"></param>
        private void SpawnLargeGun(GameObject largeGunPrefab)
        {
            SpawnGun(largeGunPrefab, largeGunSpawns);
        }

        /// <summary>
        /// Spawn any gun prefab at a specified array of potential spawn points.
        /// </summary>
        /// <param name="gunPrefab"></param>
        /// <param name="possibleSpawns"></param>
        private void SpawnGun(GameObject gunPrefab, Transform[] possibleSpawns)
        {
            int randomIndex = Random.Range(0, possibleSpawns.Length);
            GameObject instancedGun = Instantiate(gunPrefab, possibleSpawns[randomIndex].position, possibleSpawns[randomIndex].rotation);
            instancedWeapons.Add(instancedGun);
        }

        /// <summary>
        /// Destroy all instanced guns and clear instanced guns list.
        /// </summary>
        private void DespawnGuns()
        {
            foreach (GameObject weapon in instancedWeapons)
            {
                Destroy(weapon);
            }

            instancedWeapons.Clear();
        }

        /// <summary>
        /// Translate drawer slide to its open or closed position.
        /// </summary>
        private void TranslateSlide()
        {
            float currentSpeed = slideTransitionSpeed * Time.deltaTime;
            if (currentSpeed >= (slideTargetPosition - drawerSlide.position).magnitude)
            {
                currentSpeed = (slideTargetPosition - drawerSlide.position).magnitude;
            }

            drawerSlide.transform.Translate(slideDirection * currentSpeed);
        }

        /// <summary>
        /// Check if target slide position reached and call 
        /// on closed if drawer was closed. 
        /// </summary>
        private void CheckTranslationComplete()
        {
            if (drawerSlide.position == slideTargetPosition)
            {
                isTranslating = false;
            }

            if (drawerSlide.position == closedPosition.position)
            {
                DespawnGuns();
                onDrawerClose?.Invoke();
            }
        }
    }
}
