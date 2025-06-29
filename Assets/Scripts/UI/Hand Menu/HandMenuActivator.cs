using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace ShootingGallery.UI.HandMenu
{
    public class HandMenuActivator : MonoBehaviour
    {
        [SerializeField]
        private HandMenu handMenu;

        [SerializeField]
        private InputAction toggleHandMenuButton;
        [SerializeField]
        private Transform leftHandParent;
        [SerializeField] 
        private Transform rightHandParent;

        [Header("Menu Offset")]
        [SerializeField]
        private float offsetX = 0.18f;
        [SerializeField]
        private float offsetY = 0.02f;
        [SerializeField]
        private float offsetZ = 0.0f;

        private InteractorHandedness handedness;
        private Transform leftHandRef;
        private Transform rightHandRef;
        private Transform activeHandRef;

        private void Start()
        {
            CreateHandReferences();
            SwitchActiveHand(InteractorHandedness.Left);
        }

        private void Update()
        {
            MoveMenu();
        }

        private void OnEnable()
        {
            toggleHandMenuButton.Enable();
            toggleHandMenuButton.performed += ToggleShowMenu;
        }

        private void OnDisable()
        {
            toggleHandMenuButton.Disable();
            toggleHandMenuButton.performed -= ToggleShowMenu;
        }

        public void SwitchActiveHand(InteractorHandedness handedness)
        {
            this.handedness = handedness;
            SetActiveHand();
        }

        private void CreateHandReferences()
        {
            GameObject leftHandObject = new GameObject("Left Menu Ref");
            leftHandRef = leftHandObject.transform;
            leftHandRef.parent = leftHandParent;
            leftHandRef.SetLocalPositionAndRotation(leftHandRef.localPosition + new Vector3(offsetX, offsetY, offsetZ), Quaternion.identity);

            GameObject rightHandObject = new GameObject("Right Menu Ref");
            rightHandRef = rightHandObject.transform;
            rightHandRef.parent = rightHandParent;
            rightHandRef.SetLocalPositionAndRotation(rightHandRef.localPosition + new Vector3(-offsetX, offsetY, offsetZ), Quaternion.identity);
        }

        private void SetActiveHand()
        {
            if (handedness == InteractorHandedness.Left)
            {
                activeHandRef = leftHandRef;
            }
            else
            {
                activeHandRef = rightHandRef;
            }
        }

        private void MoveMenu()
        {
            if (activeHandRef == null) return;

            handMenu.transform.position = activeHandRef.transform.position;
            handMenu.transform.rotation = activeHandRef.transform.rotation;
        }

        private void ToggleShowMenu(InputAction.CallbackContext ctx)
        {
            if (!handMenu.gameObject.activeInHierarchy)
            {
                handMenu.OpenHandMenu();
            }
            else
            {
                handMenu.CloseHandMenu();
            }
        }
    }
}
