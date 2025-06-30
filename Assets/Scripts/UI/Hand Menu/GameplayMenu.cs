using ShootingGallery.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace ShootingGallery.UI.HandMenu
{
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
        }

        private void OnDisable()
        {
            showAmmoCountDropDown.onValueChanged.RemoveAllListeners();
            moveHandednessDropDown.onValueChanged.RemoveAllListeners();
            turnHandednessDropDown.onValueChanged.RemoveAllListeners();
            turnTypeDropDown.onValueChanged.RemoveAllListeners();
        }

        private void InitiateGameplayMenu()
        {
            SetAmmoCountDropDown();
            SetMoveHandednessDropDown();
            SetTurnHandednessDropDown();
            SetTurnTypeDropDown();
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

        private void SetTurnTypeDropDown()
        {
            switch (SettingsLocator.GetSettingsManager().GetTurnType())
            {
                case XR.TurnType.Continuous:
                    turnTypeDropDown.value = 0;
                    break;
                case XR.TurnType.Snap:
                    turnTypeDropDown.value = 1;
                    break;
            }
        }

        private void ShowAmmoValueChanged(int value)
        {
            SettingsLocator.GetSettingsManager().ToggleAmmoCounters(value == 0);
        }

        private void MoveHandednessChanged(int value)
        {
            InteractorHandedness handedness = value == 0 ? InteractorHandedness.Left : InteractorHandedness.Right;
            SettingsLocator.GetSettingsManager().SetMoveHandedness(handedness);
            SetTurnHandednessDropDown();
        }

        private void TurnHandednessChanged(int value)
        {
            InteractorHandedness handedness = value == 0 ? InteractorHandedness.Left : InteractorHandedness.Right;
            SettingsLocator.GetSettingsManager().SetTurnHandedness(handedness);
            SetMoveHandednessDropDown();
        }

        private void TurnTypeChanged(int value)
        {
            XR.TurnType turnType = value == 0 ? XR.TurnType.Continuous : XR.TurnType.Snap;
            SettingsLocator.GetSettingsManager().SetTurnType(turnType);
        }
    }
}
