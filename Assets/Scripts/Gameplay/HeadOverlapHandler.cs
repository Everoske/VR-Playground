using UnityEngine;

namespace ShootingGallery.Gameplay
{
    public class HeadOverlapHandler : MonoBehaviour
    {
        [SerializeField]
        private LayerMask overlapLayerMask;

        [SerializeField]
        private Renderer fadeRenderer;

        private void Update()
        {
            int maxColliders = 10;
            Collider[] hitColliders = new Collider[maxColliders];
            int numColliders = Physics.OverlapSphereNonAlloc(transform.position, 0.3f, hitColliders, overlapLayerMask);
            fadeRenderer.enabled = numColliders > 0;
        }
    }
}
