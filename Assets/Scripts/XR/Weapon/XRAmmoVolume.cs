using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace ShootingGallery.XR.Weapon
{
    /// <summary>
    /// An interactable volume that infinitely spawns magazines.
    /// </summary>
    public class XRAmmoVolume : XRBaseInteractable
    {
        [SerializeField]
        private XRGrabInteractable ammoInterablePrefab;

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            XRDirectInteractor selectInteractor = args.interactorObject as XRDirectInteractor;

            if (selectInteractor != null)
            {
                XRGrabInteractable ammoInteractable = Instantiate(ammoInterablePrefab, transform.position, Quaternion.identity);
                interactionManager.SelectEnter((IXRSelectInteractor) selectInteractor, (IXRSelectInteractable) ammoInteractable);
            }
        }
    }
}
