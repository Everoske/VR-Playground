using ShootingGallery.Game;
using ShootingGallery.Settings;
using ShootingGallery.UI;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace ShootingGallery.XR.Weapon
{
    public class XRBARifle : XRGrabInteractable
    {
        [Header("XR Bolt-Action Rifle Configuration")]
        [SerializeField]
        private XRBolt bolt;
        [SerializeField]
        private XRRifleChamber chamber;
        [SerializeField]
        private AmmoCounterUI ammoCounterUI;

        [Tooltip("Locks the bolt when rifle not held.")]
        [SerializeField]
        private bool lockBoltWhenNotHeld = true;

        [SerializeField]
        private Transform shootingOrigin;
        [SerializeField]
        private Transform ejectOrigin;
        [SerializeField]
        private LayerMask shootingLayerMask;
        [SerializeField]
        private float maxShotDistance = 2000.0f;

        [SerializeField]
        private ParticleSystem muffleFlash;
        [SerializeField]
        private ParticleSystem impactSparksPrefab;

        [SerializeField]
        private AudioClip[] shotClips;

        [Header("Haptic Feedback Settings")]
        [SerializeField]
        private ExternalHapticFeedbackPlayer hapticFeedbackPlayer;
        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float recoilAmplitude = 0.5f;
        [SerializeField]
        private float recoilDuration = 0.25f;
        [SerializeField]
        private float recoilFrequency = 0.0f;

        private RifleFireState fireState = RifleFireState.Empty;

        private AudioSource audioSource;

        protected override void Awake()
        {
            base.Awake();
            audioSource = GetComponent<AudioSource>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            bolt.onBoltPulledUp += OnBoltPulledUp;
            bolt.onBoltPulledBack += OnBoltPulledBack;
            bolt.onBoltUnobstruct += OnBoltUnobstruct;
            bolt.onBoltObstruct += OnBoltObstruct;
            bolt.onBoltPushedIn += OnBoltPushedIn;

            chamber.onLoadSequenceStart += OnAmmoLoadStart;
            chamber.onLoadSequenceEnd += OnAmmoLoadEnd;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            bolt.onBoltPulledUp -= OnBoltPulledUp;
            bolt.onBoltPulledBack -= OnBoltPulledBack;
            bolt.onBoltUnobstruct -= OnBoltUnobstruct;
            bolt.onBoltObstruct -= OnBoltObstruct;
            bolt.onBoltPushedIn -= OnBoltPushedIn;

            chamber.onLoadSequenceStart -= OnAmmoLoadStart;
            chamber.onLoadSequenceEnd -= OnAmmoLoadEnd;
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);

            if (lockBoltWhenNotHeld)
            {
                bolt.SetLockBolt(false);
            }
        }

        protected override void OnActivated(ActivateEventArgs args)
        {
            base.OnActivated(args);
            PullTrigger();
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);

            if (lockBoltWhenNotHeld && !isSelected)
            {
                bolt.SetLockBolt(true);
            }
        }

        /// <summary>
        /// Determines if gun can fire.
        /// </summary>
        /// <returns></returns>
        private bool CanFire()
        {
            return fireState == RifleFireState.LiveRoundInBarrel && bolt.IsBoltClosed();
        }

        private void EjectLiveRound()
        {
            // Create round and launch it out with a force
            fireState = RifleFireState.Empty;
        }

        private void EjectCasing()
        {
            // Create non-XR casing object and launch it out with a force
            fireState = RifleFireState.Empty;
        }

        private void PullTrigger()
        {
            if (CanFire())
            {
                ShootRifle();
            }
            else
            {
                // Play sound
            }
        }

        private void ShootRifle()
        {
            fireState = RifleFireState.CasingInBarrel;
            PlayShotClip();
            muffleFlash.Play();
            //PlayRecoilFeedback();
            //AccuracyLocator.GetAccuracyTracker().IncrementShotsFired();
            RaycastHit hit;

            if (Physics.Raycast(shootingOrigin.position, shootingOrigin.forward, out hit, maxShotDistance, shootingLayerMask, QueryTriggerInteraction.Ignore))
            {

                PlayImpactSparks(hit.point, hit.collider.transform.rotation);
                DetermineTargetHit(hit.collider.gameObject);
            }

            //SetAmmoCountUI();
        }

        private void PlayShotClip()
        {
            int clipIndex = Random.Range(0, shotClips.Length);
            audioSource.PlayOneShot(shotClips[clipIndex]);
        }

        /// <summary>
        /// Play impact sparks where the rifle hits.
        /// </summary>
        /// <param name="impactPoint">Point of impact.</param>
        /// <param name="impactRotation">Impact rotation.</param>
        private void PlayImpactSparks(Vector3 impactPoint, Quaternion impactRotation)
        {
            ParticleSystem sparks = Instantiate(impactSparksPrefab, impactPoint, impactRotation);
        }

        /// <summary>
        /// Determine if the player shot a shooting target.
        /// </summary>
        /// <param name="hitObject">Object hit by pistol.</param>
        private void DetermineTargetHit(GameObject hitObject)
        {
            if (hitObject.tag != "Target") return;
            try
            {
                ShootingTarget hitTarget = hitObject.GetComponentInParent<ShootingTarget>();
                hitTarget.HitTarget();
            }
            catch (System.Exception) { }
        }

        private void OnBoltPulledUp()
        {
            // Play sound
        }

        private void OnBoltPulledBack()
        {
            // Eject round if in barrel
            switch (fireState)
            {
                case RifleFireState.LiveRoundInBarrel:
                    EjectLiveRound();
                    break;
                case RifleFireState.CasingInBarrel:
                    EjectCasing();
                    break;
            }

            // Play sound
        }
        
        private void OnBoltUnobstruct()
        {
            chamber.SetChamberClosed(false);
        }

        private void OnBoltObstruct()
        {
            chamber.SetChamberClosed(true);
        }

        private void OnBoltPushedIn()
        {
            if (fireState == RifleFireState.Empty && chamber.HasAmmo())
            {
                chamber.ReduceAmmoCount();
                bolt.SetLockBoltMovement(true);
                fireState = RifleFireState.LiveRoundInBarrel;
            }
        }

        private void OnAmmoLoadStart()
        {
            bolt.SetLockBolt(true);
        }

        private void OnAmmoLoadEnd()
        {
            bolt.SetLockBolt(false);
        }
    }

    public enum RifleFireState
    {
        Empty,
        CasingInBarrel,
        LiveRoundInBarrel
    }
}
