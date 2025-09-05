using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace ShootingGallery.XR.Weapon
{
    /// <summary>
    /// Represents a rifle round for the bolt-action rifle.
    /// </summary>
    public class XRRifleRound : XRGrabInteractable
    {
        private XRDirectInteractor currentInteractor;

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
        /// Detach from player and destroy self.
        /// </summary>
        public void DetachAndDestroySelf()
        {
            interactionLayers = InteractionLayerMask.GetMask("Nothing");
            Destroy(gameObject);
        }
    }
}
