using ShootingGallery.XR.Weapon;
using UnityEngine;

namespace ShootingGallery.Gameplay
{
    /// <summary>
    /// Trigger volume that represents and inaccessible area and detects XRWeapons and moves them to an accessible area.
    /// </summary>
    public class WeaponRestrictedVolume : MonoBehaviour
    {
        [SerializeField]
        private Transform destination;
        [SerializeField]
        private int maxParentsToCheck = 5;

        private void OnTriggerEnter(Collider other)
        {
            FindXRWeaponInParent(1, other.gameObject);
        }

        /// <summary>
        /// Teleport XRWeapon to an accessible area to the player.
        /// </summary>
        /// <param name="weapon">Weapon to teleport.</param>
        private void TeleportWeapon(XRPistol weapon)
        {
            weapon.gameObject.SetActive(false);
            weapon.gameObject.transform.position = destination.position;
            
            if (weapon.gameObject.TryGetComponent<Rigidbody>(out Rigidbody weaponRB))
            {
                weaponRB.angularVelocity = Vector3.zero;
                weaponRB.linearVelocity = Vector3.zero;
            }

            weapon.gameObject.SetActive(true);
        }

        /// <summary>
        /// Recursive method for searching parents for an XRWeapon component.
        /// </summary>
        /// <param name="depth">Current iteration of search.</param>
        /// <param name="current">GameObject to search for XRWeapon script.</param>
        private void FindXRWeaponInParent(int depth, GameObject current)
        {
            if (depth > maxParentsToCheck) return;

            if (current.TryGetComponent<XRPistol>(out XRPistol weapon))
            {
                TeleportWeapon(weapon);
                return;
            }

            if (current.transform.parent == null) return;
            FindXRWeaponInParent(depth + 1, current.transform.parent.gameObject);
        }
    }
}
