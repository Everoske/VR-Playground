using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Collections;

public class XRMagazine : XRGrabInteractable
{
    [SerializeField]
    private uint maxAmmoCount = 12;
    [SerializeField]
    private int startingAmmo = 12;

    private XRDirectInteractor currentInteractor;
    private InteractionLayerMask defaultInteractionLayers;
    private Rigidbody rbComponent;
    private int currentAmmo;
    private Transform originalParent = null;

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

    public bool IsHeld()
    {
        return currentInteractor != null;
    }

    public void ForceDetach()
    {
        if (currentInteractor == null) return;
        SelectExitEventArgs args = new SelectExitEventArgs();
        args.interactorObject = currentInteractor;
        OnSelectExited(args);
    }

    public void LockInteraction()
    {
        interactionLayers = InteractionLayerMask.GetMask("Nothing");
        throwOnDetach = false;
        rbComponent.isKinematic = true;
    }

    public void UnlockInteraction()
    {
        interactionLayers = defaultInteractionLayers;
        throwOnDetach = true;
        rbComponent.isKinematic = false;
    }

    public bool IsKinematic()
    {
        return rbComponent.isKinematic;
    }

    public bool IsEmpty()
    {
        return currentAmmo == 0;
    }

    public void ResetParent()
    {
        transform.parent = originalParent;
    }

    public void SetAmmoToMax()
    {
        currentAmmo = (int) maxAmmoCount;
    }

    public bool IsUsed()
    {
        return IsHeld() || transform.parent != originalParent;
    }
}
