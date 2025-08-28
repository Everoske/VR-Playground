using ShootingGallery.XR.Weapon;
using UnityEngine;
using static Codice.Client.Common.EventTracking.TrackFeatureUseEvent.Features.DesktopGUI.Filters;

namespace ShootingGallery.Integrity
{
    /// <summary>
    /// Trigger volume that represents and inaccessible area and detects XRWeapons and moves them to an accessible area.
    /// </summary>
    public class WeaponRestrictedVolume : MonoBehaviour
    {
        [SerializeField]
        private int maxParentsToCheck = 5;

        private void OnTriggerEnter(Collider other)
        {
            FindGameWeaponInParent(1, other.gameObject);
        }

        /// <summary>
        /// Recursive method for searching parents for a GameWeapon component.
        /// </summary>
        /// <param name="depth">Current iteration of search.</param>
        /// <param name="current">GameObject to search for XRWeapon script.</param>
        private void FindGameWeaponInParent(int depth, GameObject current)
        {
            if (depth > maxParentsToCheck) return;

            if (current.TryGetComponent<GameWeapon>(out GameWeapon weapon))
            {
                weapon.ReturnToSpawn();
            }

            if (current.transform.parent == null) return;
            FindGameWeaponInParent(depth + 1, current.transform.parent.gameObject);
        }
    }
}
