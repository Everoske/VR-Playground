using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class XRPistol : XRGrabInteractable
{
    [SerializeField]
    private InputAction rightReleaseMagAction;
    [SerializeField]
    private InputAction leftReleaseMagAction;

    [SerializeField]
    private Transform shootingOrigin;
    [SerializeField]
    private Transform ejectOrigin;
    [SerializeField]
    private GameObject casingPrefab;

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
    private ParticleSystem smokeParticles;
    [SerializeField]
    private ParticleSystem impactSparks;

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

    [Tooltip("Locks the slider when the pistol is not held")]
    [SerializeField]
    private bool lockSlideWhenNotHeld;

    private Animator animator;
    private AudioSource audioSource;
    private bool animationPlaying = false;
    private int currentAmmo;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        DeterminePistolState();

        if (lockSlideWhenNotHeld)
        {
            slider.LockSlideForAnimation();
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        slider.onSnapForward += OnSlideSnappedForward;
        slider.onPullBack += OnSlidePulledBack;
        magWell.onMagazineInsert += MagazineAttached;
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

            if (lockSlideWhenNotHeld)
            {
                slider.UnlockSlide();
            }
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
    }

    public void PullTrigger()
    {
        if (animationPlaying) return;

        if (currentAmmo > 0 && slider.SliderIsIdle())
        {
            animationPlaying = true;
            slider.LockSlideForAnimation();
            animator.SetTrigger(currentAmmo == 1 ? "ShootLast" : "Shoot");
        }
        else
        {
            PlayAudioClip(emptyClip);
        }
    }

    // TODO: Add haptic feedback on shoot
    public void ShootPistol()
    {
        currentAmmo--;
        magWell.SetAmmoInMag(currentAmmo);
        PlayAudioClip(shootClip);
        // PlaySmokeEffect();
        RaycastHit hit;

        if (Physics.Raycast(shootingOrigin.position, shootingOrigin.forward, out hit))
        {
            PlayImpactSparks(hit.point, hit.collider.transform.rotation);
        }
    }

    public void EjectCasing()
    {
        // TODO: Implement casing ejection
    }

    private void PlayAudioClip(AudioClip clip)
    {
        if (audioSource == null) return;

        float pitch = Random.Range(minShotPitch, maxShotPitch);
        float volume = Random.Range(minShotVolume, maxShotVolume);
        audioSource.pitch = pitch;
        audioSource.volume = volume;

        audioSource.PlayOneShot(clip);
    }

    private void PlaySmokeEffect()
    {
        ParticleSystem smoke = Instantiate(smokeParticles, shootingOrigin.position, shootingOrigin.rotation);
    }

    private void PlayImpactSparks(Vector3 impactPoint, Quaternion impactRotation)
    {
        ParticleSystem sparks = Instantiate(impactSparks, impactPoint, impactRotation);
    }

    public void SetPistolAnimationEnd()
    {
        animationPlaying = false;
        DeterminePistolState();
    }

    private void DeterminePistolState()
    {
        if (currentAmmo == 0)
        {
            slider.SetEmptyState(true);
            animator.SetBool("ShotReady", false);
        }
    }

    private void MagazineAttached()
    {
        slider.SetEmptyState(false);
    }

    private void OnReleaseMagazinePressed(InputAction.CallbackContext ctx)
    {
        // Leave one round in chamber if loaded
        int tempAmmo = currentAmmo;
        currentAmmo = currentAmmo > 1 ? 1 : 0;
        magWell.SetAmmoInMag(tempAmmo - currentAmmo);
        magWell.ReleaseMagazine();
    }

    private void OnSlidePulledBack()
    {
        PlayAudioClip(pulledBackClip);
        // TODO: Eject round when in chamber
        if (currentAmmo > 0)
        {
            currentAmmo--;
            EjectCasing();
            DeterminePistolState();
        }
    }

    private void OnSlideSnappedForward()
    {
        PlayAudioClip(snappedForwardClip);

        if (currentAmmo == 0 && magWell.HasLoadedMagazine())
        {
            currentAmmo = magWell.GetAmmoInMag();
            animator.SetBool("ShotReady", true);
        }
    }
}
