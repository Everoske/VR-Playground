using UnityEngine;
using UnityEngine.Events;

namespace ShootingGallery.XR.Weapon
{
    /// <summary>
    /// Represents a magazine well in a firearm. Allows for insertion of a magazine
    /// and provides information on ammunition capacity.
    /// </summary>
    public class XRMagazineWell : MonoBehaviour
    {
        [Tooltip("Attach point for magazine.")]
        [SerializeField]
        private Transform attachPoint;
        [Tooltip("Adding a magazine here will automatically attach it on game start.")]
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

        private void OnTriggerEnter(Collider other)
        {
            if (activeMagazine != null) return;

            XRMagazine magazine;

            if (other.transform.parent != null && other.transform.parent.TryGetComponent<XRMagazine>(out magazine))
            {
                AssignMagazine(magazine);
            }
        }

        /// <summary>
        /// Assign magazine as active magazine if held by the player and
        /// force detach it from the player's interactor.
        /// </summary>
        /// <param name="magazine">Magazine to assign and detach.</param>
        private void AssignMagazine(XRMagazine magazine)
        {
            if (!magazine.IsHeld()) return;
            activeMagazine = magazine;
            activeMagazine.ForceDetach();
        }

        /// <summary>
        /// Attach active magazine to attach point and play magazine insertion animation.
        /// </summary>
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

        /// <summary>
        /// Called from animation when magazine release animation has completed.
        /// </summary>
        public void MagazineReleased()
        {
            activeMagazine.ResetParent();
            activeMagazine.UnlockInteraction();
            activeMagazine = null;
            isMagazineAttached = false;
            onMagazineReleased?.Invoke();
        }

        /// <summary>
        /// Called from animation when magazine insert animation has completed.
        /// </summary>
        public void MagazineInsert()
        {
            onMagazineInsert?.Invoke();
        }

        /// <summary>
        /// Release active magazine.
        /// </summary>
        public void ReleaseMagazine()
        {
            if (activeMagazine == null) return;
            animator.SetBool("HasMag", false);
        }

        /// <summary>
        /// Determine if a full magazine is inserted into the magazine well.
        /// </summary>
        /// <returns></returns>
        public bool HasLoadedMagazine()
        {
            return activeMagazine != null && activeMagazine.CurrentAmmo > 0;
        }

        /// <summary>
        /// Return the ammunition in the active magazine.
        /// </summary>
        /// <returns></returns>
        public int GetAmmoInMag()
        {
            if (activeMagazine == null) return 0;

            return (int)activeMagazine.CurrentAmmo;
        }

        /// <summary>
        /// Set the ammunition in the active magazine.
        /// </summary>
        /// <param name="amount"></param>
        public void SetAmmoInMag(int amount)
        {
            if (activeMagazine == null) return;
            activeMagazine.CurrentAmmo = amount;
        }

        /// <summary>
        /// Use one round in the active magazine.
        /// </summary>
        public void ConsumeRound()
        {
            if (activeMagazine == null || activeMagazine.IsEmpty()) return;
            activeMagazine.CurrentAmmo = activeMagazine.CurrentAmmo - 1;
        }
    }
}
