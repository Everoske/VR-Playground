using TMPro;
using UnityEngine;

namespace ShootingGallery.UI
{
    public class AmmoCounterUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text ammoText;

        public void SetAmmoCount(int count)
        {
            ammoText.text = count.ToString();
        }
    }
}
