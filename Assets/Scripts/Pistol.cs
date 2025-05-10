using UnityEngine;

public class Pistol : MonoBehaviour
{
    [SerializeField]
    private Transform shootingOrigin;

    [SerializeField]
    [Range(0f, 1f)]
    private float randomPitchMin = 0.5f;

    [SerializeField]
    private int maxAmmo = 12;

    [SerializeField]
    private ParticleSystem impactSparks;

    private Animator animator;
    private AudioSource audioSource;
    private bool animationPlaying = false;
    private int currentAmmo;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        currentAmmo = maxAmmo;
    }

    public void PullTrigger()
    {
        if (animationPlaying) return;

        if (currentAmmo > 0)
        {
            animationPlaying = true;
            animator.SetTrigger("Shoot");
        }
        else
        {
            // Play empty sound
        }
    }

    public void Shoot()
    {
        currentAmmo--;
        PlayShootingSound();
        RaycastHit hit;

        if (Physics.Raycast(shootingOrigin.position, shootingOrigin.forward, out hit))
        {
            PlayImpactSparks(hit.point, hit.collider.transform.rotation);

            if (hit.collider.tag != "Target") return;
            try
            {
                ShootingTarget hitTarget = hit.collider.GetComponentInParent<ShootingTarget>();
                hitTarget.HitTarget();
            }
            catch (System.Exception) { }
        }
    }

    private void PlayShootingSound()
    {
        if (audioSource == null) return;

        float pitch = Random.Range(randomPitchMin, 1.0f);
        audioSource.pitch = pitch;

        audioSource.Play();
    }

    private void PlayImpactSparks(Vector3 impactPoint, Quaternion impactRotation)
    {
        ParticleSystem sparks = Instantiate(impactSparks, impactPoint, impactRotation);
    }

    public void SetPistolAnimationEnd()
    {
        animationPlaying = false;
        if (currentAmmo == 0)
        {
            animator.SetTrigger("Empty");
        }
    }
}
