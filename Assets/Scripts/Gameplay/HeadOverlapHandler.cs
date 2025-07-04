using UnityEngine;

namespace ShootingGallery.Gameplay
{
    public class HeadOverlapHandler : MonoBehaviour
    {
        [SerializeField]
        private LayerMask overlapLayerMask;

        [SerializeField]
        private float overlapRadius = 0.25f;

        [SerializeField]
        private float maxFadeDistance = 0.05f;

        [SerializeField]
        private float detectionInterval = 0.1f;

        [SerializeField]
        private Renderer fadeRenderer;

        private float detectionTimer = 0.0f;
        private bool activelyFading = false;

        private void Update()
        {
            detectionTimer += Time.deltaTime;

            if (detectionTimer > detectionInterval || activelyFading)
            {
                DetectHeadOverlap();
                detectionTimer = 0.0f;
            }
        }

        private void DetectHeadOverlap()
        {
            int maxColliders = 10;
            Collider[] hitColliders = new Collider[maxColliders];
            int numColliders = Physics.OverlapSphereNonAlloc(transform.position, overlapRadius, hitColliders, overlapLayerMask);
            fadeRenderer.enabled = numColliders > 0;
            activelyFading = numColliders > 0;

            if (numColliders > 0)
            {
                CalculateFade(hitColliders, numColliders);
            }
        }

        private void CalculateFade(Collider[] hitColliders, int numColliders)
        {
            float closestDistance = 0.25f;

            for (int i = 0; i < numColliders; i++)
            {
                Vector3 closestPoint = hitColliders[i].ClosestPointOnBounds(transform.position);
                float colliderDistance = Vector3.Distance(transform.position, closestPoint);
                if (colliderDistance < closestDistance)
                {
                    closestDistance = colliderDistance;
                }
            }

            float fadePercentage = (overlapRadius - closestDistance) / maxFadeDistance;
            SetFade(Mathf.Lerp(0.0f, 1.0f, fadePercentage));
        }

        private void SetFade(float alpha)
        {
            fadeRenderer.material.SetFloat("_Alpha", alpha);
        }
    }
}
