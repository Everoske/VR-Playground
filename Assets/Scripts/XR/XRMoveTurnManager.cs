using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

using ShootingGallery.Settings;
using ShootingGallery.Enums;

namespace ShootingGallery.XR
{
    /// <summary>
    /// Toggle which hands are used to move and turn as well as which turn provider to use.
    /// </summary>
    public class XRMoveTurnManager : MonoBehaviour
    {
        [SerializeField]
        private SnapTurnProvider snapTurnProvider;
        [SerializeField]
        private ContinuousTurnProvider continuousTurnProvider;
        [SerializeField]
        private DynamicMoveProvider moveProvider;

        [Header("Input Action References:")]
        [SerializeField]
        private InputActionReference rightHandMoveInput;
        [SerializeField]
        private InputActionReference leftHandMoveInput;
        [SerializeField]
        private InputActionReference rightHandSnapTurnInput;
        [SerializeField]
        private InputActionReference leftHandSnapTurnInput;
        [SerializeField]
        private InputActionReference rightHandContinuousTurnInput;
        [SerializeField]
        private InputActionReference leftHandContinuousTurnInput;

        private void Start()
        {
            SetTurnType(SettingsLocator.GetSettingsManager().GetTurnType());
            SetTurnHandedness(SettingsLocator.GetSettingsManager().GetTurnHandedness());
            SetMoveHandedness(SettingsLocator.GetSettingsManager().GetMoveHandedness());

            // Subscribe to events
            SettingsLocator.GetSettingsManager().onTurnTypeChanged += SetTurnType;
            SettingsLocator.GetSettingsManager().onTurnHandednessChanged += SetTurnHandedness;
            SettingsLocator.GetSettingsManager().onMoveHandednessChanged += SetMoveHandedness;
        }

        private void OnDisable()
        {
            // Unsubscribe from events
            SettingsLocator.GetSettingsManager().onTurnTypeChanged -= SetTurnType;
            SettingsLocator.GetSettingsManager().onTurnHandednessChanged -= SetTurnHandedness;
            SettingsLocator.GetSettingsManager().onMoveHandednessChanged -= SetMoveHandedness;
        }

        /// <summary>
        /// Set which controller the player uses to move.
        /// </summary>
        /// <param name="handedness">Right or left controller.</param>
        public void SetMoveHandedness(InteractorHandedness handedness)
        {
            switch (handedness)
            {
                case InteractorHandedness.Right:
                    moveProvider.leftHandMoveInput.inputActionReference = null;
                    moveProvider.rightHandMoveInput.inputActionReference = rightHandMoveInput;
                    break;
                case InteractorHandedness.Left:
                    moveProvider.rightHandMoveInput.inputActionReference = null;
                    moveProvider.leftHandMoveInput.inputActionReference = leftHandMoveInput;
                    break;
            }
        }

        /// <summary>
        /// Set which controller the player uses to turn.
        /// </summary>
        /// <param name="handedness">Right or left controller.</param>
        public void SetTurnHandedness(InteractorHandedness handedness)
        {
            SetSnapTurnHandedness(handedness);
            SetContinuousTurnHandedness(handedness);
        }

        /// <summary>
        /// Set what type of turn provider the player uses to turn.
        /// </summary>
        /// <param name="turnType">Continuous or snap turn provider.</param>
        public void SetTurnType(TurnType turnType)
        {
            switch (turnType)
            {
                case TurnType.Snap:
                    continuousTurnProvider.enabled = false;
                    snapTurnProvider.enabled = true;
                    break;
                case TurnType.Continuous:
                    snapTurnProvider.enabled = false;
                    continuousTurnProvider.enabled = true;
                    break;
            }
        }

        /// <summary>
        /// Set which controller is used for snap turning.
        /// </summary>
        /// <param name="handedness">Right or left controller.</param>
        private void SetSnapTurnHandedness(InteractorHandedness handedness)
        {
            switch (handedness)
            {
                case InteractorHandedness.Right:
                    snapTurnProvider.leftHandTurnInput.inputActionReference = null;
                    snapTurnProvider.rightHandTurnInput.inputActionReference = rightHandSnapTurnInput;
                    break;
                case InteractorHandedness.Left:
                    snapTurnProvider.rightHandTurnInput.inputActionReference = null;
                    snapTurnProvider.leftHandTurnInput.inputActionReference = leftHandSnapTurnInput;
                    break;
            }
        }

        /// <summary>
        /// Set which controller is used for continuous turning.
        /// </summary>
        /// <param name="handedness">Right or left controller.</param>
        private void SetContinuousTurnHandedness(InteractorHandedness handedness)
        {
            switch (handedness)
            {
                case InteractorHandedness.Right:
                    continuousTurnProvider.leftHandTurnInput.inputActionReference = null;
                    continuousTurnProvider.rightHandTurnInput.inputActionReference = rightHandContinuousTurnInput;
                    break;
                case InteractorHandedness.Left:
                    continuousTurnProvider.rightHandTurnInput.inputActionReference = null;
                    continuousTurnProvider.leftHandTurnInput.inputActionReference = leftHandContinuousTurnInput;
                    break;
            }
        }
    }
}
