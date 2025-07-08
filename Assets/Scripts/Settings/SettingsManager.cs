using ShootingGallery.Data;
using ShootingGallery.XR;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace ShootingGallery.Settings
{
    /// <summary>
    /// Manages in-game settings including loading and saving settings.
    /// </summary>
    public class SettingsManager : MonoBehaviour, ISaveable
    {
        [SerializeField]
        private AudioMixer audioMixer;
        [SerializeField]
        private XRMoveTurnManager moveTurnManager;

        private float masterVolume = 0.0f;
        private float musicVolume = 0.0f;
        private float sfxVolume = 0.0f;
        private bool showAmmoCounter = false;
        private TurnType turnType = TurnType.Snap;
        private InteractorHandedness turnHandedness = InteractorHandedness.Right;
        private InteractorHandedness moveHandedness = InteractorHandedness.Left;
        private InteractorHandedness menuHandedness = InteractorHandedness.Left;

        public UnityAction<bool> onShowAmmoCounterChanged;
        public UnityAction<InteractorHandedness> onChangeMenuHandedness;

        public float GetMasterVolume() => masterVolume;
        public float GetMusicVolume() => musicVolume;
        public float GetSFXVolume() => sfxVolume;
        public bool GetShowAmmoCounter() => showAmmoCounter;
        public TurnType GetTurnType() => turnType;
        public InteractorHandedness GetTurnHandedness() => turnHandedness;
        public InteractorHandedness GetMoveHandedness() => moveHandedness;

        public InteractorHandedness GetMenuHandedness() => menuHandedness;  

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

        public void SetTurnType(TurnType turnType)
        {
            this.turnType = turnType;
            moveTurnManager.SetTurnType(turnType);
        }

        public void SetTurnHandedness(InteractorHandedness handedness)
        {
            if (turnHandedness == handedness) return;
            turnHandedness = handedness;
            moveHandedness = handedness == InteractorHandedness.Right ? InteractorHandedness.Left : InteractorHandedness.Right;

            moveTurnManager.SetTurnHandedness(turnHandedness);
            moveTurnManager.SetMoveHandedness(moveHandedness);
        }

        public void SetMoveHandedness(InteractorHandedness handedness)
        {
            if (moveHandedness == handedness) return;
            moveHandedness = handedness;
            turnHandedness = handedness == InteractorHandedness.Right ? InteractorHandedness.Left : InteractorHandedness.Right;

            moveTurnManager.SetMoveHandedness(moveHandedness);
            moveTurnManager.SetTurnHandedness(turnHandedness);
        }

        public void SetHandedness(InteractorHandedness turnHandedness, InteractorHandedness moveHandedness)
        {
            this.turnHandedness = turnHandedness;
            this.moveHandedness = moveHandedness;

            moveTurnManager.SetTurnHandedness(this.turnHandedness);
            moveTurnManager.SetMoveHandedness(this.moveHandedness);
        }

        public void SetMenuHandedness(InteractorHandedness handedness)
        {
            menuHandedness = handedness;
            onChangeMenuHandedness?.Invoke(handedness);
        }

        public void LoadData(GameData data)
        {
            SetMasterVolume(data.masterVolume);
            SetMusicVolume(data.musicVolume);
            SetSFXVolume(data.sfxVolume);
            ToggleAmmoCounters(data.showAmmoCounter);
            SetTurnType(data.turnType);
            SetHandedness(data.turnHandedness, data.moveHandedness);
            SetMenuHandedness(data.menuHandedness);
        }

        public void SaveData(ref GameData data)
        {
            data.masterVolume = masterVolume;
            data.musicVolume = musicVolume;
            data.sfxVolume = sfxVolume;
            data.showAmmoCounter = showAmmoCounter;
            data.turnType = turnType;
            data.turnHandedness = turnHandedness;
            data.moveHandedness = moveHandedness;
            data.menuHandedness = menuHandedness;
        }
    }
}
