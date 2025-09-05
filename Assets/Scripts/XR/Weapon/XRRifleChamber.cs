using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace ShootingGallery.XR.Weapon
{
    /// <summary>
    /// Represents the chamber where bullets are loaded into the 
    /// bolt-action rifle.
    /// </summary>
    public class XRRifleChamber : MonoBehaviour
    {
        [SerializeField]
        private Transform chamberTransform;

        [SerializeField]
        private GameObject[] bullets;

        [SerializeField]
        private GameObject loadBulletMesh;

        [SerializeField]
        private float bulletSpacing = 0.006f;

        [SerializeField]
        private float initialDisplacement = 0.0015f;

        [SerializeField]
        private float displacementDuration = 0.25f;

        private Animator chamberAnimator;

        private int roundsInChamber = 0;

        private float defaultYPosition;
        private float lastYPosition;
        private float targetYPosition;
        private float displacementTimer = 0.0f;

        private bool loadingRound = false;
        private bool chamberClosed = true;

        public UnityAction onLoadSequenceStart;
        public UnityAction onLoadSequenceEnd;

        private void Awake()
        {
            chamberAnimator = GetComponent<Animator>();
        }

        private void Start()
        {
            defaultYPosition = chamberTransform.localPosition.y;
            lastYPosition = defaultYPosition;
            targetYPosition = defaultYPosition;
        }

        private void Update()
        {
            if (chamberTransform.localPosition.y != targetYPosition)
            {
                displacementTimer += Time.deltaTime;
                float newYPosition = Mathf.Lerp(lastYPosition, targetYPosition, displacementTimer / displacementDuration);
                chamberTransform.localPosition = new Vector3(
                    chamberTransform.localPosition.x,
                    newYPosition,
                    chamberTransform.localPosition.z
                    );

                if (newYPosition == targetYPosition)
                {
                    loadingRound = false;
                    onLoadSequenceEnd?.Invoke();
                }
            }
        }

        /// <summary>
        /// Set the displacement for the ammo group and increment rounds in chamber.
        /// </summary>
        public void OnLoadAnimationComplete()
        {
            loadBulletMesh.SetActive(false);
            bullets[roundsInChamber].SetActive(true);
            
            if (roundsInChamber == 0)
            {
                lastYPosition = defaultYPosition;
                targetYPosition = chamberTransform.localPosition.y - (initialDisplacement + bulletSpacing);
            }
            else
            {
                lastYPosition = targetYPosition;
                targetYPosition = chamberTransform.localPosition.y - bulletSpacing;
            }

            displacementTimer = 0.0f;
            roundsInChamber++;
        }

        /// <summary>
        /// Reduce ammo visuals and move the chamber group up. 
        /// </summary>
        public void ReduceAmmoCount()
        {
            if (roundsInChamber == 0 || !chamberClosed) return;

            float newPositionY = chamberTransform.localPosition.y + bulletSpacing;

            if (roundsInChamber == 1)
            {
                newPositionY += initialDisplacement;
            }

            bullets[roundsInChamber - 1].SetActive(false);
            lastYPosition = targetYPosition;
            targetYPosition = newPositionY;
            displacementTimer = 0.0f;
            roundsInChamber--;

            onLoadSequenceStart?.Invoke();
        }

        /// <summary>
        /// Prevent ammo from being inserted into chamber when
        /// bolt is obstructing chamber.
        /// </summary>
        /// <param name="closed"></param>
        public void SetChamberClosed(bool closed)
        {
            chamberClosed = closed;
        }

        /// <summary>
        /// Determines if ammo is in chamber.
        /// </summary>
        /// <returns></returns>
        public bool HasAmmo()
        {
            return roundsInChamber > 0;
        }

        /// <summary>
        /// Begin animation where rifle round is inserted into chamber.
        /// </summary>
        /// <param name="rifleRound"></param>
        private void InsertRifleRound(XRRifleRound rifleRound)
        {
            loadingRound = true;
            onLoadSequenceStart?.Invoke();

            rifleRound.DetachAndDestroySelf();
            loadBulletMesh.SetActive(true);

            if (roundsInChamber == 0)
            {
                chamberAnimator.SetTrigger("LoadInitial");
            }
            else if (roundsInChamber % 2 == 0)
            {
                chamberAnimator.SetTrigger("LoadRight");
            }
            else
            {
                chamberAnimator.SetTrigger("LoadLeft");
            }
        }

        /// <summary>
        /// Determines whether round can be added to chamber 
        /// or not.
        /// </summary>
        /// <returns></returns>
        private bool CanLoadRound()
        {
            return roundsInChamber < bullets.Length &&
                !loadingRound && !chamberClosed;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!CanLoadRound()) return;

            XRRifleRound rifleRound;

            if (other.transform.TryGetComponent<XRRifleRound>(out rifleRound))
            {
                if (!rifleRound.IsHeld()) return;
                InsertRifleRound(rifleRound);
            }
        }
    }
}
