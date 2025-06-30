using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

namespace ShootingGallery.XR
{
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

        public void SetTurnHandedness(InteractorHandedness handedness)
        {
            SetSnapTurnHandedness(handedness);
            SetContinuousTurnHandedness(handedness);
        }

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

    public enum TurnType
    {
        Snap,
        Continuous
    }
}
