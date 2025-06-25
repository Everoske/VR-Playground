using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace ShootingGallery.XR.Weapon
{
    /// <summary>
    /// Represents a magazine that can be interacted with by the player and inserted into a firearm.
    /// </summary>
    public class XRMagazine : XRGrabInteractable
    {
        [SerializeField]
        private uint maxAmmoCount = 17;
        [SerializeField]
        private int startingAmmo = 17;

        private XRDirectInteractor currentInteractor;
        private InteractionLayerMask defaultInteractionLayers;
        private Rigidbody rbComponent;
        private int currentAmmo;
        private Transform originalParent = null;

        /// <summary>
        /// Current ammo in the magazine.
        /// </summary>
        public int CurrentAmmo
        {
            get => currentAmmo;
            set
            {
                currentAmmo = value > maxAmmoCount ? (int)maxAmmoCount : value >= 0 ? value : 0;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            rbComponent = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            defaultInteractionLayers = interactionLayers;
            CurrentAmmo = startingAmmo;
            originalParent = transform.parent;
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            XRDirectInteractor directInteractor = args.interactorObject as XRDirectInteractor;

            if (directInteractor != null)
            {
                currentInteractor = directInteractor;
            }
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            XRDirectInteractor directInteractor = args.interactorObject as XRDirectInteractor;

            if (directInteractor != null && directInteractor == currentInteractor)
            {
                currentInteractor = null;
            }
        }

        /// <summary>
        /// Determines if magazine is held by player.
        /// </summary>
        /// <returns></returns>
        public bool IsHeld()
        {
            return currentInteractor != null;
        }

        /// <summary>
        /// Forcibly detaches the magazine from its current interactor so the magazine
        /// can be loaded into a firearm.
        /// </summary>
        public void ForceDetach()
        {
            if (currentInteractor == null) return;
            SelectExitEventArgs args = new SelectExitEventArgs();
            args.interactorObject = currentInteractor;
            OnSelectExited(args);
        }

        /// <summary>
        /// Prevents the magazine from being interacted with by the player
        /// and ensures it is not impacted by physics.
        /// </summary>
        public void LockInteraction()
        {
            interactionLayers = InteractionLayerMask.GetMask("Nothing");
            throwOnDetach = false;
            rbComponent.isKinematic = true;
        }

        /// <summary>
        /// Allows the magazine to be interacted with by the player and 
        /// impacted by physics.
        /// </summary>
        public void UnlockInteraction()
        {
            interactionLayers = defaultInteractionLayers;
            throwOnDetach = true;
            rbComponent.isKinematic = false;
        }

        /// <summary>
        /// Determines if the magazine is kinematic.
        /// </summary>
        /// <returns></returns>
        public bool IsKinematic()
        {
            return rbComponent.isKinematic;
        }

        /// <summary>
        /// Is the magazine empty.
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return currentAmmo == 0;
        }

        /// <summary>
        /// Resets the parent of the magazine.
        /// </summary>
        public void ResetParent()
        {
            transform.parent = originalParent;
        }

        /// <summary>
        /// Sets current ammo to max.
        /// </summary>
        public void SetAmmoToMax()
        {
            currentAmmo = (int)maxAmmoCount;
        }

        /// <summary>
        /// Determines if the magazine is full.
        /// </summary>
        /// <returns></returns>
        public bool HasMaxAmmo()
        {
            return currentAmmo >= maxAmmoCount;
        }

        /// <summary>
        /// Determines if the magazine is held by the player or inserted into 
        /// a firearm. 
        /// </summary>
        /// <returns></returns>
        public bool IsUsed()
        {
            return IsHeld() || transform.parent != originalParent;
        }
    }
}
