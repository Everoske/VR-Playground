using UnityEngine;
using UnityEngine.Audio;

namespace ShootingGallery.Settings
{
    public class SettingsManager : MonoBehaviour
    {
        [SerializeField]
        private AudioMixer audioMixer;

        private float masterVolume = 0.0f;
        private float musicVolume = 0.0f;
        private float sfxVolume = 0.0f;
        bool showAmmoCounter = true;

        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp(volume, -80.0f, 20.0f);
            audioMixer.SetFloat("masterVolume", masterVolume);
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp(volume, -80.0f, 20.0f);
            audioMixer.SetFloat("musicVolume", musicVolume);
        }

        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp(volume, -80.0f, 20.0f);
            audioMixer.SetFloat("sfxVolume", sfxVolume);
        }

        public void ToggleAmmoCounters(bool show)
        {
            showAmmoCounter = show;
            UpdateAmmoCounters();
        }

        private void UpdateAmmoCounters()
        {
            // TODO: Hide all ammo counters for every weapon in the game. Perhaps use an interface.
        }
    }
}
