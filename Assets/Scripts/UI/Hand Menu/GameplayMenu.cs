using ShootingGallery.Settings;
using ShootingGallery.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace ShootingGallery.UI.HandMenu
{
    /// <summary>
    /// Menu that allows player to set in-game settings.
    /// </summary>
    public class GameplayMenu : MonoBehaviour
    {
        [SerializeField]
        private TMP_Dropdown showAmmoCountDropDown;
        [SerializeField]
        private TMP_Dropdown moveHandednessDropDown;
        [SerializeField]
        private TMP_Dropdown turnHandednessDropDown;
        [SerializeField]
        private TMP_Dropdown turnTypeDropDown;
        [SerializeField]
        private TMP_Dropdown menuHandednessDropDown;

        private void Start()
        {
            InitiateGameplayMenu();
        }

        private void OnEnable()
        {
            showAmmoCountDropDown.onValueChanged.AddListener(ShowAmmoValueChanged);
            moveHandednessDropDown.onValueChanged.AddListener(MoveHandednessChanged);
            turnHandednessDropDown.onValueChanged.AddListener(TurnHandednessChanged);
            turnTypeDropDown.onValueChanged.AddListener(TurnTypeChanged);
            menuHandednessDropDown.onValueChanged.AddListener(MenuHandednessChanged);
        }

        private void OnDisable()
        {
            showAmmoCountDropDown.onValueChanged.RemoveAllListeners();
            moveHandednessDropDown.onValueChanged.RemoveAllListeners();
            turnHandednessDropDown.onValueChanged.RemoveAllListeners();
            turnTypeDropDown.onValueChanged.RemoveAllListeners();
            menuHandednessDropDown.onValueChanged.RemoveAllListeners();
        }

        /// <summary>
        /// Set initial values for the dropdown menus based on save data.
        /// </summary>
        private void InitiateGameplayMenu()
        {
            SetAmmoCountDropDown();
            SetMoveHandednessDropDown();
            SetTurnHandednessDropDown();
            SetTurnTypeDropDown();
            SetMenuHandednessDropDown();
        }

        /// <summary>
        /// Set the show ammo count dropdown based on save data.
        /// </summary>
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

        /// <summary>
        /// Set the move handedness dropdown based on save data.
        /// </summary>
        private void SetMoveHandednessDropDown()
        {
            switch (SettingsLocator.GetSettingsManager().GetMoveHandedness())
            {
                case InteractorHandedness.Left:
                    moveHandednessDropDown.value = 0;
                    break;
                case InteractorHandedness.Right:
                    moveHandednessDropDown.value = 1;
                    break;
            }
        }

        /// <summary>
        /// Set the turn handedness dropdown based on save data.
        /// </summary>
        private void SetTurnHandednessDropDown()
        {
            switch (SettingsLocator.GetSettingsManager().GetTurnHandedness())
            {
                case InteractorHandedness.Left:
                    turnHandednessDropDown.value = 0;
                    break;
                case InteractorHandedness.Right:
                    turnHandednessDropDown.value = 1;
                    break;
            }
        }

        /// <summary>
        /// Set the turn type dropdown based on save data.
        /// </summary>
        private void SetTurnTypeDropDown()
        {
            switch (SettingsLocator.GetSettingsManager().GetTurnType())
            {
                case TurnType.Continuous:
                    turnTypeDropDown.value = 0;
                    break;
                case TurnType.Snap:
                    turnTypeDropDown.value = 1;
                    break;
            }
        }

        /// <summary>
        /// Set the menu handedness dropdown based on save data.
        /// </summary>
        private void SetMenuHandednessDropDown()
        {
            switch (SettingsLocator.GetSettingsManager().GetMenuHandedness())
            {
                case InteractorHandedness.Left:
                    menuHandednessDropDown.value = 0;
                    break;
                case InteractorHandedness.Right:
                    menuHandednessDropDown.value = 1;
                    break;
            }
        }

        /// <summary>
        /// Toggle showing ammo counters when the dropdown value changes.
        /// </summary>
        /// <param name="value">Index of dropdown menu.</param>
        private void ShowAmmoValueChanged(int value)
        {
            SettingsLocator.GetSettingsManager().ToggleAmmoCounters(value == 0);
        }

        /// <summary>
        /// Toggle move handedness when the dropdown value changes.
        /// </summary>
        /// <param name="value">Index of dropdown menu.</param>
        private void MoveHandednessChanged(int value)
        {
            InteractorHandedness handedness = value == 0 ? InteractorHandedness.Left : InteractorHandedness.Right;
            SettingsLocator.GetSettingsManager().SetMoveHandedness(handedness);
            SetTurnHandednessDropDown();
        }

        /// <summary>
        /// Toggle turn handedness when the dropdown value changes.
        /// </summary>
        /// <param name="value">Index of dropdown menu.</param>
        private void TurnHandednessChanged(int value)
        {
            InteractorHandedness handedness = value == 0 ? InteractorHandedness.Left : InteractorHandedness.Right;
            SettingsLocator.GetSettingsManager().SetTurnHandedness(handedness);
            SetMoveHandednessDropDown();
        }

        /// <summary>
        /// Toggle move type when the dropdown value changes.
        /// </summary>
        /// <param name="value">Index of dropdown menu.</param>
        private void TurnTypeChanged(int value)
        {
            TurnType turnType = value == 0 ? TurnType.Continuous : TurnType.Snap;
            SettingsLocator.GetSettingsManager().SetTurnType(turnType);
        }

        /// <summary>
        /// Toggle which hand the hand menu follows when the dropdown value changes.
        /// </summary>
        /// <param name="value">Index of dropdown menu.</param>
        private void MenuHandednessChanged(int value)
        {
            InteractorHandedness handedness = value == 0 ? InteractorHandedness.Left : InteractorHandedness.Right;
            SettingsLocator.GetSettingsManager().SetMenuHandedness(handedness);
        }
    }
}
