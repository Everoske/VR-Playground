using ShootingGallery.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ShootingGallery.UI.HandMenu
{
    public class GameplayMenu : MonoBehaviour
    {
        [SerializeField]
        private TMP_Dropdown showAmmoCountDropDown;

        private void Start()
        {
            InitiateGameplayMenu();
        }

        private void OnEnable()
        {
            showAmmoCountDropDown.onValueChanged.AddListener(ShowAmmoValueChanged);
        }

        private void OnDisable()
        {
            showAmmoCountDropDown.onValueChanged.RemoveAllListeners();
        }

        private void InitiateGameplayMenu()
        {
            SetAmmoCountDropDown();
        }

        private void SetAmmoCountDropDown()
        {
            if (SettingsLocator.GetSettingsManager().GetShowAmmoCounter())
            {
                showAmmoCountDropDown.value = 0;
            }
            else
            {
                showAmmoCountDropDown.value = 1;
            }
        }

        private void ShowAmmoValueChanged(int value)
        {
            SettingsLocator.GetSettingsManager().ToggleAmmoCounters(value == 0);
        }
    }
}
