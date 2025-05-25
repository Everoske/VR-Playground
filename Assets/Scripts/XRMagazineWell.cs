using UnityEngine;
using UnityEngine.Events;

public class XRMagazineWell : MonoBehaviour
{
    [SerializeField]
    private Transform attachPoint;
    [SerializeField]
    private XRMagazine activeMagazine;

    public UnityAction onMagazineReleased;
    public UnityAction onMagazineInsert;
    private Animator animator;
    private bool isMagazineAttached = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (activeMagazine != null)
        {
            AttachMagazine();
        }
    }

    private void Update()
    {
        if (activeMagazine != null && !isMagazineAttached)
        {
            AttachMagazine();
        }
    }

    // Attach Magazine to attach point and play animation
    private void OnTriggerEnter(Collider other)
    {
        if (activeMagazine != null) return;

        XRMagazine magazine;

        if (other.transform.parent != null && other.transform.parent.TryGetComponent<XRMagazine>(out magazine))
        {
            if (!magazine.IsHeld()) return;
            activeMagazine = magazine;
            activeMagazine.ForceDetach();
        }
    }

    private void AttachMagazine()
    {
        isMagazineAttached = activeMagazine.transform.parent == attachPoint && activeMagazine.IsKinematic();
        if (isMagazineAttached)
        {
            animator.SetBool("HasMag", true);
            return;
        }
        activeMagazine.LockInteraction();
        activeMagazine.transform.parent = attachPoint;
        activeMagazine.transform.localPosition = Vector3.zero;
        activeMagazine.transform.localRotation = Quaternion.identity;
    }

    // Call after release magazine animation completes
    public void MagazineReleased()
    {
        activeMagazine.ResetParent();
        activeMagazine.UnlockInteraction();
        activeMagazine = null;
        isMagazineAttached = false;
        onMagazineReleased?.Invoke();
    }

    // Call after insert magazine animation completes
    public void MagazineInsert()
    {
        onMagazineInsert?.Invoke();
    }

    // Call from Pistol Class
    public void ReleaseMagazine()
    {
        if (activeMagazine == null) return;
        animator.SetBool("HasMag", false);
    }

    public bool HasLoadedMagazine()
    {
        return activeMagazine != null && activeMagazine.CurrentAmmo > 0;
    }

    public int GetAmmoInMag()
    {
        if (activeMagazine == null) return 0;

        return (int)activeMagazine.CurrentAmmo;
    }

    public void SetAmmoInMag(int amount)
    {
        if (activeMagazine == null) return;
        activeMagazine.CurrentAmmo = amount;
    }

    public void ConsumeRound()
    {
        if (activeMagazine == null || activeMagazine.IsEmpty()) return;
        activeMagazine.CurrentAmmo = activeMagazine.CurrentAmmo - 1;
    }
}
