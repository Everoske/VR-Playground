using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using ShootingGallery.Game;
using ShootingGallery.UI;
using ShootingGallery.Settings;

namespace ShootingGallery.XR.Weapon
{
    /// <summary>
    /// Represents a semi-automatic pistol the player hold, fire, and reload.
    /// </summary>
    public class XRPistol : XRGrabInteractable
    {
        [Header("XR Pistol Settings")]
        [SerializeField]
        private InputAction rightReleaseMagAction;
        [SerializeField]
        private InputAction leftReleaseMagAction;

        [SerializeField]
        private Transform shootingOrigin;
        [SerializeField]
        private Transform ejectOrigin;
        [SerializeField]
        private LayerMask shootingLayerMask;
        [SerializeField]
        private float maxShotDistance = 2000.0f;

        [SerializeField]
        [Range(0f, 3f)]
        private float minShotPitch = 0.6f;
        [SerializeField]
        [Range(0f, 3f)]
        private float maxShotPitch = 1.2f;
        [SerializeField]
        [Range(0f, 1f)]
        private float minShotVolume = 0.8f;
        [SerializeField]
        [Range(0f, 1f)]
        private float maxShotVolume = 1.0f;

        [SerializeField]
        private ParticleSystem muffleFlash;
        [SerializeField]
        private ParticleSystem impactSparksPrefab;

        [SerializeField]
        private AudioClip shootClip;
        [SerializeField]
        private AudioClip emptyClip;
        [SerializeField]
        private AudioClip pulledBackClip;
        [SerializeField]
        private AudioClip snappedForwardClip;

        [SerializeField]
        private XRPistolSlider slider;
        [SerializeField]
        private XRMagazineWell magWell;

        [SerializeField]
        private AmmoCounterUI ammoCounterUI;

        [Tooltip("Locks the slider when the pistol is not held.")]
        [SerializeField]
        private bool lockSlideWhenNotHeld;

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

        private Animator animator;
        private AudioSource audioSource;
        private bool animationPlaying = false;
        private bool roundInChamber = false;
        private bool showAmmoCounter = false;
        private InteractorHandedness handedness = InteractorHandedness.None;

        protected override void Awake()
        {
            base.Awake();
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            DeterminePistolState(false);

            if (lockSlideWhenNotHeld)
            {
                slider.LockSlideForAnimation();
            }

            InitializeAmmoCounter();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            slider.onSnapForward += OnSlideSnappedForward;
            slider.onPullBack += OnSlidePulledBack;
            magWell.onMagazineInsert += MagazineAttached;
            magWell.onMagazineReleased += MagazineReleased;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            rightReleaseMagAction.Disable();
            rightReleaseMagAction.performed -= OnReleaseMagazinePressed;
            leftReleaseMagAction.Disable();
            leftReleaseMagAction.performed -= OnReleaseMagazinePressed;
            slider.onSnapForward -= OnSlideSnappedForward;
            slider.onPullBack -= OnSlidePulledBack;
            magWell.onMagazineInsert -= MagazineAttached;
            magWell.onMagazineReleased -= MagazineReleased;
            SettingsLocator.GetSettingsManager().onShowAmmoCounterChanged -= ShowAmmoCountChanged;
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);

            XRDirectInteractor interactor = args.interactorObject as XRDirectInteractor;

            if (interactor != null)
            {
                switch (interactor.handedness)
                {
                    case InteractorHandedness.Left:
                        // Subscribe to left hand
                        leftReleaseMagAction.Enable();
                        leftReleaseMagAction.performed += OnReleaseMagazinePressed;
                        break;
                    case InteractorHandedness.Right:
                        // Subscribe to right hand
                        rightReleaseMagAction.Enable();
                        rightReleaseMagAction.performed += OnReleaseMagazinePressed;
                        break;
                }

                handedness = interactor.handedness;

                if (lockSlideWhenNotHeld)
                {
                    slider.UnlockSlide();
                }

                ActivateAmmoCountUI();
            }
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            rightReleaseMagAction.Disable();
            rightReleaseMagAction.performed -= OnReleaseMagazinePressed;
            leftReleaseMagAction.Disable();
            leftReleaseMagAction.performed -= OnReleaseMagazinePressed;

            if (lockSlideWhenNotHeld)
            {
                slider.LockSlideForAnimation();
            }

            ammoCounterUI.gameObject.SetActive(false);
            handedness = InteractorHandedness.None;
        }

        protected override void OnActivated(ActivateEventArgs args)
        {
            base.OnActivated(args);
            PullTrigger();
        }

        /// <summary>
        /// Play pistol firing animation if ready to fire. Otherwise, play empty sound effect.
        /// </summary>
        public void PullTrigger()
        {
            if (animationPlaying) return;

            if (roundInChamber && slider.SliderIsIdle())
            {
                animationPlaying = true;
                slider.LockSlideForAnimation();
                animator.SetTrigger(!magWell.HasLoadedMagazine() ? "ShootLast" : "Shoot");
            }
            else
            {
                PlayAudioClip(emptyClip);
            }
        }

        /// <summary>
        /// Called from the pistol firing animation. Play firing effects, consume ammunition, check 
        /// if anything hit, adjust accuracy, and set ammo counter UI.
        /// </summary>
        public void ShootPistol()
        {
            roundInChamber = magWell.HasLoadedMagazine();
            magWell.ConsumeRound();
            PlayAudioClip(shootClip);
            PlayMuffleFlash();
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
        /// Eject a casing object from the pistol when fired.
        /// </summary>
        public void EjectCasing()
        {
            // TODO: Implement casing ejection
        }

        /// <summary>
        /// Play audio clip with a random pitch and volume.
        /// </summary>
        /// <param name="clip">Audio clip to play.</param>
        private void PlayAudioClip(AudioClip clip)
        {
            if (audioSource == null) return;

            float pitch = Random.Range(minShotPitch, maxShotPitch);
            float volume = Random.Range(minShotVolume, maxShotVolume);
            audioSource.pitch = pitch;
            audioSource.volume = volume;
            audioSource.PlayOneShot(clip);
        }
        
        /// <summary>
        /// Play muffle flash when the pistol is fired.
        /// </summary>
        private void PlayMuffleFlash()
        {
            muffleFlash.Play();
        }

        /// <summary>
        /// Play impact sparks where the pistol hits.
        /// </summary>
        /// <param name="impactPoint">Point of impact.</param>
        /// <param name="impactRotation">Impact rotation.</param>
        private void PlayImpactSparks(Vector3 impactPoint, Quaternion impactRotation)
        {
            ParticleSystem sparks = Instantiate(impactSparksPrefab, impactPoint, impactRotation);
        }

        /// <summary>
        /// Called from the pistol firing animation when it ends.
        /// </summary>
        public void SetPistolAnimationEnd()
        {
            animationPlaying = false;
            slider.UnlockSlide();
            DeterminePistolState(true);
        }

        /// <summary>
        /// Determine whether the pistol can be fired and whether to
        /// engage the slide stop for the pistol slide.
        /// </summary>
        /// <param name="pistolFired">Was the pistol just fired?</param>
        private void DeterminePistolState(bool pistolFired)
        {
            if (!roundInChamber && !magWell.HasLoadedMagazine())
            {
                animator.SetBool("ShotReady", false);

                if (pistolFired)
                {
                    slider.EngageSlideStop();
                }
            }
        }

        /// <summary>
        /// Set the ammo counter UI when magazine is attached.
        /// </summary>
        private void MagazineAttached()
        {
            if (roundInChamber)
            {
                SetAmmoCountUI();
            }
        }

        
        private void MagazineReleased()
        {
            SetAmmoCountUI();
        }

        /// <summary>
        /// Release magazine when the player presses the release button.
        /// </summary>
        /// <param name="ctx"></param>
        private void OnReleaseMagazinePressed(InputAction.CallbackContext ctx)
        {
            magWell.ReleaseMagazine();
        }

        /// <summary>
        /// Play pulled back audio. Eject a round if present in the pistol's chamber.
        /// </summary>
        private void OnSlidePulledBack()
        {
            PlayAudioClip(pulledBackClip);

            if (roundInChamber)
            {
                roundInChamber = false;
                EjectCasing();
                DeterminePistolState(false);
            }
        }

        /// <summary>
        /// Play snapped forward audio. Reload pistol if no round in chamber
        /// and with a loaded magazine inserted.
        /// </summary>
        private void OnSlideSnappedForward()
        {
            PlayAudioClip(snappedForwardClip);

            if (!roundInChamber && magWell.HasLoadedMagazine())
            {
                magWell.ConsumeRound();
                roundInChamber = true;
                animator.SetBool("ShotReady", true);
                SetAmmoCountUI();
            }
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
        /// Play haptic feedback when the pistol is fired.
        /// </summary>
        private void PlayRecoilFeedback()
        {
            if (hapticFeedbackPlayer == null) return;
            switch (handedness)
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
        /// Update the pistol's UI ammo counter.
        /// </summary>
        private void SetAmmoCountUI()
        {
            ammoCounterUI.SetAmmoCount(roundInChamber ?
                magWell.GetAmmoInMag() + 1 : 0);
        }

        private void ActivateAmmoCountUI()
        {
            if (!showAmmoCounter) return;
            SetAmmoCountUI();
            ammoCounterUI.gameObject.SetActive(true);
        }

        private void ShowAmmoCountChanged(bool show)
        {
            showAmmoCounter = show;

            if (handedness != InteractorHandedness.None)
            {
                ActivateAmmoCountUI();
            }
        }

        private void InitializeAmmoCounter()
        {
            ammoCounterUI.gameObject.SetActive(false);
            showAmmoCounter = SettingsLocator.GetSettingsManager().GetShowAmmoCounter();
            SettingsLocator.GetSettingsManager().onShowAmmoCounterChanged += ShowAmmoCountChanged;
        }
    }
}
