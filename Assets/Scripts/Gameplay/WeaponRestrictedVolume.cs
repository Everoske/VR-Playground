using ShootingGallery.XR.Weapon;
using UnityEngine;

namespace ShootingGallery.Gameplay
{
    public class WeaponRestrictedVolume : MonoBehaviour
    {
        [SerializeField]
        private Transform destination;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<XRPistol>(out XRPistol weapon))
            {
                TeleportWeapon(weapon);
            }
        }

        private void TeleportWeapon(XRPistol weapon)
        {
            weapon.gameObject.SetActive(false);
            weapon.gameObject.transform.position = destination.position;
            weapon.gameObject.SetActive(true);
        }
    }
}
