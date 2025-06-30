using UnityEngine;
using UnityEngine.UI;
using ShootingGallery.Settings;

namespace ShootingGallery.UI.HandMenu
{
    public class SoundMenu : MonoBehaviour
    {
        [SerializeField]
        private Slider mainVolumeSlider;
        [SerializeField]
        private Slider musicVolumeSlider;
        [SerializeField]
        private Slider sfxVolumeSlider;

        private void Start()
        {
            InitializeSliders();
        }

        private void OnEnable()
        {
            mainVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
            sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        private void OnDisable()
        {
            mainVolumeSlider.onValueChanged.RemoveAllListeners();
            musicVolumeSlider.onValueChanged.RemoveAllListeners();
            sfxVolumeSlider.onValueChanged.RemoveAllListeners();
        }

        private void InitializeSliders()
        {
            mainVolumeSlider.value = SettingsLocator.GetSettingsManager().GetMasterVolume();
            musicVolumeSlider.value = SettingsLocator.GetSettingsManager().GetMusicVolume();
            sfxVolumeSlider.value = SettingsLocator.GetSettingsManager().GetSFXVolume();
        }

        private void SetMasterVolume(float volume)
        {
            if (SettingsLocator.GetSettingsManager() == null) return;
            SettingsLocator.GetSettingsManager().SetMasterVolume(volume);
        }

        private void SetMusicVolume(float volume)
        {
            if (SettingsLocator.GetSettingsManager() == null) return;
            SettingsLocator.GetSettingsManager().SetMusicVolume(volume);
        }

        private void SetSFXVolume(float volume)
        {
            if (SettingsLocator.GetSettingsManager() == null) return;
            SettingsLocator.GetSettingsManager().SetSFXVolume(volume);
        }
    }
}
