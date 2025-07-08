using UnityEngine;
using UnityEngine.Events;

namespace ShootingGallery.Game
{
    public class ShootingTarget : MonoBehaviour
    {
        [SerializeField]
        private Material activeMaterial;

        [SerializeField]
        private Material inactiveMaterial;

        [Tooltip("Represents which axes to rotate a target once it has been shot using Euler angles.")]
        [SerializeField]
        private Vector3 rotationAxes = new Vector3(-90.0f, 0.0f, 0.0f);

        // TODO: Implement smooth rotation
        [SerializeField]
        private float rotationSpeed = 0.15f; 

        private MeshRenderer meshRenderer;
        private bool isTargetActive = true;

        private TargetType targetType;
        private ITargetHitNotify targetHitNotify;

        public TargetType TargetType
        {
            get => targetType;
            set => targetType = value;
        }

        public ITargetHitNotify TargetHitNotify
        {
            get => targetHitNotify;
            set => targetHitNotify = value;
        }

        private void Awake()
        {
            meshRenderer = GetComponentInChildren<MeshRenderer>();
        }

        private void Update()
        {
            if (isTargetActive) return;

            Quaternion currentRotation = transform.rotation;
            Quaternion targetRotation = Quaternion.Euler(rotationAxes.x, rotationAxes.y, rotationAxes.z);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, Time.deltaTime / rotationSpeed);
        }

        public void HitTarget()
        {
            if (!isTargetActive) return;

            targetHitNotify.OnTargetHit(targetType);
            meshRenderer.material = inactiveMaterial;
            isTargetActive = false;
        }

        public void ResetTarget()
        {
            if (isTargetActive) return;

            isTargetActive = true;
            targetHitNotify = null;
            meshRenderer.material = activeMaterial;
            transform.rotation = Quaternion.identity;
        }
    }
}
