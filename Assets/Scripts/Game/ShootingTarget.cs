using UnityEngine;

using ShootingGallery.Enums;

namespace ShootingGallery.Game
{
    public class ShootingTarget : MonoBehaviour
    {
        [SerializeField]
        private Material activeMaterial; // REMOVE

        [SerializeField]
        private Material inactiveMaterial; // REMOVE

        [Tooltip("Represents which axes to rotate a target once it has been shot using Euler angles.")]
        [SerializeField]
        private Vector3 rotationAxes = new Vector3(-90.0f, 0.0f, 0.0f);

        [Tooltip("Amount of time to rotate after being hit.")]
        [SerializeField]
        private float rotationTime = 1.0f;

        [SerializeField]
        private GameObject targetMesh;

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
            meshRenderer = targetMesh.GetComponent<MeshRenderer>();
            poolParent = transform.parent;
        }

        private void Update()
        {
            if (isTargetActive) return;
            if (rotateTimer > rotationTime) return;
            RotateTargetMesh();
        }

        /// <summary>
        /// Deactivate target and start rotating the hit mesh to show it is no longer active.
        /// </summary>
        public void HitTarget()
        {
            if (!isTargetActive) return;

            targetHitNotify?.OnTargetHit(targetType);
            meshRenderer.material = inactiveMaterial; // REMOVE
            isTargetActive = false;
            rotateTimer = 0.0f;
        }

        /// <summary>
        /// Reset the mesh of target.
        /// </summary>
        public void ResetTarget()
        {
            if (isTargetActive) return;

            isTargetActive = true;
            targetHitNotify = null;
            meshRenderer.material = activeMaterial; // REMOVE
            targetMesh.transform.rotation = Quaternion.identity;
        }

        /// <summary>
        /// Rotate the hittable mesh part of the target after it has been hit successfully.
        /// </summary>
        private void RotateTargetMesh()
        {
            Quaternion currentRotation = targetMesh.transform.rotation;
            Quaternion targetRotation = Quaternion.Euler(rotationAxes.x, rotationAxes.y, rotationAxes.z);
            rotateTimer += Time.deltaTime;
            targetMesh.transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotateTimer / rotationTime);
        }
    }
}
