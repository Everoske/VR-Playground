using UnityEngine;
using UnityEngine.Events;

namespace ShootingGallery.Game
{
    /// <summary>
    /// Handles spawning ammo volumes in selected ammo drawers.
    /// </summary>
    public class AmmoDrawer : MonoBehaviour
    {
        [SerializeField]
        private Transform ammoVolumePosition;

        [SerializeField]
        private Transform drawerSlide;
        [SerializeField]
        private Transform closedPosition;
        [SerializeField]
        private Transform openPosition;
        [SerializeField]
        private float slideTransitionSpeed = 1.0f;

        private Vector3 slideTargetPosition;
        private Vector3 slideDirection;
        private bool isTranslating = false;

        private GameObject ammoVolume;

        public UnityAction onDrawerClosed;

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
        /// Spawn given ammo volume prefab.
        /// </summary>
        /// <param name="volumePrefab"></param>
        public void SpawnAmmoVolume(GameObject volumePrefab)
        {
            ammoVolume = Instantiate(volumePrefab, ammoVolumePosition.position, ammoVolumePosition.rotation);
            OpenDrawer();
        }

        /// <summary>
        /// Begin closing drawer to then despawn ammo volume. 
        /// </summary>
        public void InitiateRemoveAmmoVolume()
        {
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
        /// Check if drawer is closed.
        /// </summary>
        /// <returns></returns>
        public bool IsDrawerClosed()
        {
            return !isTranslating && drawerSlide.position == closedPosition.position;
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
                DespawnAmmoVolume();
                onDrawerClosed?.Invoke();
            }
        }

        /// <summary>
        /// Despawn current ammo volume.
        /// </summary>
        private void DespawnAmmoVolume()
        {
            Destroy(ammoVolume);
            ammoVolume = null;
        }
    }
}
