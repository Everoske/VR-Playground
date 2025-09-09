using ShootingGallery.Game;
using ShootingGallery.Settings;
using ShootingGallery.UI;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

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
        private Transform trigger;
        [SerializeField]
        private float triggerPressedZRotation = 22.0f;
        [SerializeField]
        private AmmoCounterUI ammoCounterUI;

        [Tooltip("Locks the bolt when rifle not held.")]
        [SerializeField]
        private bool lockBoltWhenNotHeld = true;

        [Header("XR Bolt-Action Rifle: Shooting Configuration")]
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

        [Header("XR Bolt-Action Rifle: Ejection Settings")]
        [SerializeField]
        private Rigidbody liveRoundPrefab;
        [SerializeField]
        private Rigidbody emptyCasingPrefab;
        [SerializeField]
        private float ejectionImpulseForce = 2.0f;

        [Header("XR Bolt-Action Rifle: Audio Settings")]
        [SerializeField]
        private AudioClip[] shotClips;
        [SerializeField]
        private AudioClip boltPushedInClip;
        [SerializeField]
        private AudioClip boltPulledUpClip;
        [SerializeField]
        private AudioClip emptyTriggerPullClip;
        [SerializeField]
        private AudioClip casingEjectedClip;

        [SerializeField]
        private AudioSource fireAudioSource;
        [SerializeField]
        private AudioSource boltFoleyAudioSource;
        [SerializeField]
        private AudioSource ejectAudioSource;

        [Header("XR Bolt-Action Rifle: Haptic Feedback Settings")]
        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float recoilAmplitude = 0.5f;
        [SerializeField]
        private float recoilDuration = 0.25f;
        [SerializeField]
        private float recoilFrequency = 0.0f;

        private RifleFireState fireState = RifleFireState.Empty;

        private XRDirectInteractor mainInteractor = null;
        private XRDirectInteractor secondaryInteractor = null;
        private ExternalHapticFeedbackPlayer hapticFeedbackPlayer;

        private bool showAmmoCounter;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            hapticFeedbackPlayer = ExternalHapticFeedbackPlayerLocator.GetHapticFeedbackPlayer();
            InitializeAmmoCounter();
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

            XRDirectInteractor interactor = args.interactorObject as XRDirectInteractor;
            
            if (interactor != null)
            {
                HandleSelect(interactor);
            }

            
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);

            XRDirectInteractor interactor = args.interactorObject as XRDirectInteractor;

            if (interactor != null)
            {
                HandleSelectExit(interactor);
            }
        }

        protected override void OnActivated(ActivateEventArgs args)
        {
            base.OnActivated(args);

            XRDirectInteractor interactor = args.interactorObject as XRDirectInteractor;

            if (interactor != null && interactor.handedness == mainInteractor.handedness)
            {
                PullTrigger();
            }
        }

        protected override void OnDeactivated(DeactivateEventArgs args)
        {
            base.OnDeactivated(args);

            ResetTrigger();
        }

        /// <summary>
        /// Determines if gun can fire.
        /// </summary>
        /// <returns></returns>
        private bool CanFire()
        {
            return fireState == RifleFireState.LiveRoundInBarrel && bolt.IsBoltClosed();
        }

        /// <summary>
        /// Ejects a live round that can be used again.
        /// </summary>
        private void EjectLiveRound()
        {
            Rigidbody round = Instantiate(liveRoundPrefab, ejectOrigin.position, Quaternion.Euler(90.0f, 0.0f, 0.0f));
            round.AddForce(ejectOrigin.forward * ejectionImpulseForce, ForceMode.Impulse);
            fireState = RifleFireState.Empty;

            ejectAudioSource.PlayOneShot(casingEjectedClip);
        }

        /// <summary>
        /// Ejects an empty casing that despawns after a set amount of time.
        /// </summary>
        private void EjectCasing()
        {
            Rigidbody round = Instantiate(emptyCasingPrefab, ejectOrigin.position, Quaternion.Euler(90.0f, 0.0f, 0.0f));
            round.AddForce(ejectOrigin.forward * ejectionImpulseForce, ForceMode.Impulse);
            Destroy(round.gameObject, 5.0f);
            fireState = RifleFireState.Empty;

            ejectAudioSource.PlayOneShot(casingEjectedClip);
        }

        /// <summary>
        /// Handles the logic for a trigger pull.
        /// </summary>
        private void PullTrigger()
        {
            if (CanFire())
            {
                ShootRifle();
            }
            else
            {
                fireAudioSource.PlayOneShot(emptyTriggerPullClip);
            }

            trigger.localRotation = Quaternion.Euler(0.0f, 0.0f, triggerPressedZRotation);
        }

        /// <summary>
        /// Resets the trigger transform.
        /// </summary>
        private void ResetTrigger()
        {
            trigger.localRotation = Quaternion.identity;
        }

        /// <summary>
        /// Handles the logic for shooting the rifle.
        /// </summary>
        private void ShootRifle()
        {
            fireState = RifleFireState.CasingInBarrel;
            PlayShotClip();
            muffleFlash.Play();
            PlayRecoilFeedback();
            AccuracyLocator.GetAccuracyTracker().IncrementShotsFired();
            RaycastHit hit;

            if (Physics.Raycast(shootingOrigin.position, shootingOrigin.forward, out hit, maxShotDistance, shootingLayerMask, QueryTriggerInteraction.Ignore))
            {

                PlayImpactSparks(hit.point, hit.collider.transform.rotation);
                DetermineTargetHit(hit.collider.gameObject);
            }

            SetAmmoCountUI();
        }

        /// <summary>
        /// Play random shot clip.
        /// </summary>
        private void PlayShotClip()
        {
            int clipIndex = Random.Range(0, shotClips.Length);
            fireAudioSource.PlayOneShot(shotClips[clipIndex]);
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

        /// <summary>
        /// Play haptic feedback when the rifle is fired.
        /// </summary>
        private void PlayRecoilFeedback()
        {
            if (hapticFeedbackPlayer == null) return;

            PlayRecoilFeedback(mainInteractor);
            PlayRecoilFeedback(secondaryInteractor);
        }

        /// <summary>
        /// Play haptic feedback for a single interactor.
        /// </summary>
        /// <param name="interactor"></param>
        private void PlayRecoilFeedback(XRDirectInteractor interactor)
        {
            if (interactor == null) return;

            switch (interactor.handedness)
            {
                case InteractorHandedness.Right:
                    hapticFeedbackPlayer.SendRightHapticImpulse(recoilAmplitude, recoilDuration, recoilFrequency);
                    break;
                case InteractorHandedness.Left:
                    hapticFeedbackPlayer.SendLeftHapticImpulse(recoilAmplitude, recoilDuration, recoilFrequency);
                    break;
            }
        }

        /// <summary>
        /// Determine interactor state upon select.
        /// </summary>
        /// <param name="interactor"></param>
        private void HandleSelect(XRDirectInteractor interactor)
        {
            if (mainInteractor == null)
            {
                mainInteractor = interactor;
                ActivateAmmoCountUI();
            }
            else
            {
                secondaryInteractor = interactor;
            }

            if (lockBoltWhenNotHeld)
            {
                bolt.SetLockBolt(false);
            }
        }

        /// <summary>
        /// Determine interactor state upon interactor exit.
        /// </summary>
        /// <param name="interactor"></param>
        private void HandleSelectExit(XRDirectInteractor interactor)
        {
            if (interactor == secondaryInteractor)
            {
                secondaryInteractor = null;
            }
            else if (interactor == mainInteractor && secondaryInteractor != null)
            {
                mainInteractor = secondaryInteractor;
                secondaryInteractor = null;
            }
            else if (interactor == mainInteractor && secondaryInteractor == null)
            {
                mainInteractor = null;
                if (lockBoltWhenNotHeld)
                {
                    bolt.SetLockBolt(true);
                }
                DeactivateAmmoCountUI();
            }  
        }

        /// <summary>
        /// Play sound when player pulls bolt up.
        /// </summary>
        private void OnBoltPulledUp()
        {
            if (!boltFoleyAudioSource.isPlaying)
            {
                boltFoleyAudioSource.PlayOneShot(boltPulledUpClip);
            }
        }

        /// <summary>
        /// Eject casing or bullet if present in the barrel when the player
        /// pulls the bolt all the way back.
        /// </summary>
        private void OnBoltPulledBack()
        {
            switch (fireState)
            {
                case RifleFireState.LiveRoundInBarrel:
                    EjectLiveRound();
                    break;
                case RifleFireState.CasingInBarrel:
                    EjectCasing();
                    break;
            }
            SetAmmoCountUI();
        }
        
        /// <summary>
        /// Allow for ammo insertion when the bolt no longer
        /// obstructs the ammo chamber.
        /// </summary>
        private void OnBoltUnobstruct()
        {
            chamber.SetChamberClosed(false);
        }

        /// <summary>
        /// Present ammo insertion when the bolt obstructs the ammo
        /// chamber.
        /// </summary>
        private void OnBoltObstruct()
        {
            chamber.SetChamberClosed(true);
        }

        /// <summary>
        /// Play sound when bolt pushed in and add ammo to barrel
        /// if present in the ammo chamber.
        /// </summary>
        private void OnBoltPushedIn()
        {
            if (!boltFoleyAudioSource.isPlaying)
            {
                boltFoleyAudioSource.PlayOneShot(boltPushedInClip);
            }

            if (fireState == RifleFireState.Empty && chamber.HasAmmo())
            {
                chamber.ReduceAmmoCount();
                bolt.SetLockBoltMovement(true);
                fireState = RifleFireState.LiveRoundInBarrel;
            }
        }

        /// <summary>
        /// Lock the bolt when loading ammo animation begins to play.
        /// </summary>
        private void OnAmmoLoadStart()
        {
            bolt.SetLockBolt(true);
        }

        /// <summary>
        /// Unlock the bolt when loading ammo animation finishes.
        /// </summary>
        private void OnAmmoLoadEnd()
        {
            bolt.SetLockBolt(false);
            SetAmmoCountUI();
        }

        /// <summary>
        /// Set ammo UI to reflect ammo in rifle.
        /// </summary>
        private void SetAmmoCountUI()
        {
            int ammoCount = chamber.GetAmmoCount();

            if (fireState == RifleFireState.LiveRoundInBarrel)
            {
                ammoCount++;
            }

            ammoCounterUI.SetAmmoCount(ammoCount);
        }

        /// <summary>
        /// Updates and displays ammo counter.
        /// </summary>
        private void ActivateAmmoCountUI()
        {
            if (!showAmmoCounter) return;
            SetAmmoCountUI();
            ammoCounterUI.gameObject.SetActive(true);
        }

        /// <summary>
        /// Hides ammo counter.
        /// </summary>
        private void DeactivateAmmoCountUI()
        {
            ammoCounterUI.gameObject.SetActive(false);
        }

        /// <summary>
        /// When player changes show ammo UI, display or hide
        /// ammo counter if gun currently held by player.
        /// </summary>
        /// <param name="show"></param>
        private void OnShowAmmoChanged(bool show)
        {
            showAmmoCounter = show;

            if (mainInteractor != null)
            {
                if (showAmmoCounter)
                {
                    ActivateAmmoCountUI();
                }
                else
                {
                    DeactivateAmmoCountUI();
                }
            }
        }

        /// <summary>
        /// Initialize ammo counter based on saved game settings.
        /// </summary>
        private void InitializeAmmoCounter()
        {
            ammoCounterUI.gameObject.SetActive(false);
            if (SettingsLocator.GetSettingsManager() != null)
            {
                showAmmoCounter = SettingsLocator.GetSettingsManager().GetShowAmmoCounter();
                SettingsLocator.GetSettingsManager().onShowAmmoCounterChanged += OnShowAmmoChanged;
            }
        }
    }

    /// <summary>
    /// Enum for managing the rifle's fire state.
    /// </summary>
    public enum RifleFireState
    {
        Empty,
        CasingInBarrel,
        LiveRoundInBarrel
    }
}
