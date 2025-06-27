using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

namespace ShootingGallery.Settings
{
    public class SettingsManager : MonoBehaviour
    {
        [SerializeField]
        private AudioMixer audioMixer;

        private float masterVolume = 0.0f;
        private float musicVolume = 0.0f;
        private float sfxVolume = 0.0f;
        private bool showAmmoCounter = true;

        public UnityAction<bool> onShowAmmoCounterChanged;

        public float GetMasterVolume() => masterVolume;
        public float GetMusicVolume() => musicVolume;
        public float GetSFXVolume() => sfxVolume;
        public bool GetShowAmmoCounter() => showAmmoCounter;

        private void Awake()
        {
            SettingsLocator.Provide(this);
        }

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
            onShowAmmoCounterChanged?.Invoke(showAmmoCounter);
        }
    }
}
