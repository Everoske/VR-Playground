using ShootingGallery.Game;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using static Codice.Client.Common.Connection.AskCredentialsToUser;

namespace ShootingGallery.XR.Weapon
{
    public class GameWeapon : MonoBehaviour, IGameWeapon
    {
        private Transform spawnPoint;
        private XRGrabInteractable weaponInteractable;
        private Rigidbody weaponRB;

        private void Awake()
        {
            weaponInteractable = GetComponent<XRGrabInteractable>();
            weaponRB = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Return weapon to spawn point.
        /// </summary>
        public void ReturnToSpawn()
        {
            if (spawnPoint == null) return;
            
            if (!weaponRB.isKinematic)
            {
                weaponRB.angularVelocity = Vector3.zero;
                weaponRB.linearVelocity = Vector3.zero;
            }

            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
        }

        /// <summary>
        /// Set spawn for weapon.
        /// </summary>
        /// <param name="spawnPosition"></param>
        public void SetSpawnPosition(Transform spawnPoint)
        {
            this.spawnPoint = spawnPoint;
        }

        /// <summary>
        /// Disable interaction and terminate any interaction.
        /// </summary>
        public void DisableAndTerminateInteraction()
        {
            weaponInteractable.interactionLayers = InteractionLayerMask.GetMask("Nothing");
            weaponInteractable.throwOnDetach = false;
            weaponRB.isKinematic = true;
        }
    }
}
