using UnityEngine;

using ShootingGallery.Enums;

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

        [SerializeField]
        private float rotationSpeed = 0.15f; 

        private MeshRenderer meshRenderer;
        private bool isTargetActive = true;

        private TargetType targetType;
        private ITargetHitNotify targetHitNotify;
        private float rotateTimer = 0.0f;
        private Transform poolParent = null;

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

        public Transform PoolParent => poolParent;

        private void Awake()
        {
            meshRenderer = GetComponentInChildren<MeshRenderer>();
            poolParent = transform.parent;
        }

        private void Update()
        {
            if (isTargetActive) return;
            if (rotateTimer > rotationSpeed) return;
            RotateTarget();
        }

        public void HitTarget()
        {
            if (!isTargetActive) return;

            targetHitNotify?.OnTargetHit(targetType);
            meshRenderer.material = inactiveMaterial;
            isTargetActive = false;
            rotateTimer = 0.0f;
        }

        public void ResetTarget()
        {
            if (isTargetActive) return;

            isTargetActive = true;
            targetHitNotify = null;
            meshRenderer.material = activeMaterial;
            transform.rotation = Quaternion.identity;
        }

        private void RotateTarget()
        {
            Quaternion currentRotation = transform.rotation;
            Quaternion targetRotation = Quaternion.Euler(rotationAxes.x, rotationAxes.y, rotationAxes.z);
            rotateTimer += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotateTimer / rotationSpeed);
        }
    }
}
