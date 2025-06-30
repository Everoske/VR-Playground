using UnityEngine;
using UnityEngine.UI;
using ShootingGallery.Settings;

namespace ShootingGallery.UI.HandMenu
{
    /// <summary>
    /// Menu that allows player's to set sound settings.
    /// </summary>
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

        /// <summary>
        /// Initiate sliders with saved volumes.
        /// </summary>
        private void InitializeSliders()
        {
            mainVolumeSlider.value = SettingsLocator.GetSettingsManager().GetMasterVolume();
            musicVolumeSlider.value = SettingsLocator.GetSettingsManager().GetMusicVolume();
            sfxVolumeSlider.value = SettingsLocator.GetSettingsManager().GetSFXVolume();
        }

        /// <summary>
        /// Set the master volume.
        /// </summary>
        /// <param name="volume">Volume to set (between -80.0 and 20.0).</param>
        private void SetMasterVolume(float volume)
        {
            if (SettingsLocator.GetSettingsManager() == null) return;
            SettingsLocator.GetSettingsManager().SetMasterVolume(volume);
        }

        /// <summary>
        /// Set the music volume.
        /// </summary>
        /// <param name="volume">Volume to set (between -80.0 and 20.0).</param>
        private void SetMusicVolume(float volume)
        {
            if (SettingsLocator.GetSettingsManager() == null) return;
            SettingsLocator.GetSettingsManager().SetMusicVolume(volume);
        }

        /// <summary>
        /// Set the SFX volume.
        /// </summary>
        /// <param name="volume">Volume to set (between -80.0 and 20.0).</param>
        private void SetSFXVolume(float volume)
        {
            if (SettingsLocator.GetSettingsManager() == null) return;
            SettingsLocator.GetSettingsManager().SetSFXVolume(volume);
        }
    }
}
