using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField]
    private Transform shootingOrigin;

    [SerializeField]
    [Range(0f, 1f)]
    private float randomPitchMin = 0.5f;

    [SerializeField]
    private ParticleSystem impactSparks;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Shoot()
    {
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
}
