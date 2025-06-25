using TMPro;
using UnityEngine;

namespace ShootingGallery.UI
{
    /// <summary>
    /// Represents a world-space UI ammo counter for weapons.
    /// </summary>
    public class AmmoCounterUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text ammoText;

        /// <summary>
        /// Display current ammo count.
        /// </summary>
        /// <param name="count"></param>
        public void SetAmmoCount(int count)
        {
            ammoText.text = count.ToString();
        }
    }
}
