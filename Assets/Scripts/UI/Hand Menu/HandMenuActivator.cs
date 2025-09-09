using ShootingGallery.Settings;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace ShootingGallery.UI.HandMenu
{
    /// <summary>
    /// Allows the player to open the hand menu and moves the menu with the 
    /// player's selected hand.
    /// </summary>
    public class HandMenuActivator : MonoBehaviour
    {
        [SerializeField]
        private HandMenuController handMenu;

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
            SwitchActiveHand(SettingsLocator.GetSettingsManager().GetMenuHandedness());
            SettingsLocator.GetSettingsManager().onHandMenuHandednessChanged += SwitchActiveHand;
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
            SettingsLocator.GetSettingsManager().onHandMenuHandednessChanged -= SwitchActiveHand;
        }

        /// <summary>
        /// Switch which hand the hand menu follows.
        /// </summary>
        /// <param name="handedness">Hand to follow.</param>
        public void SwitchActiveHand(InteractorHandedness handedness)
        {
            this.handedness = handedness;
            SetActiveHand();
        }

        /// <summary>
        /// Create hand references for the hand menu to follow and add them to each tracked hand object.
        /// </summary>
        private void CreateHandReferences()
        {
            GameObject leftHandObject = new GameObject("Left Menu Ref");
            leftHandRef = leftHandObject.transform;
            leftHandRef.parent = leftHandParent;
            leftHandRef.SetLocalPositionAndRotation(new Vector3(offsetX, offsetY, offsetZ), Quaternion.identity);

            GameObject rightHandObject = new GameObject("Right Menu Ref");
            rightHandRef = rightHandObject.transform;
            rightHandRef.parent = rightHandParent;
            rightHandRef.SetLocalPositionAndRotation(new Vector3(-offsetX, offsetY, offsetZ), Quaternion.identity);
        }

        /// <summary>
        /// Set the active hand reference for the hand menu to follow.
        /// </summary>
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

        /// <summary>
        /// Move the hand menu with the active hand reference.
        /// </summary>
        private void MoveMenu()
        {
            if (activeHandRef == null) return;

            handMenu.transform.position = activeHandRef.transform.position;
            handMenu.transform.rotation = activeHandRef.transform.rotation;
        }

        /// <summary>
        /// Show or hide the hand menu.
        /// </summary>
        /// <param name="ctx"></param>
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
