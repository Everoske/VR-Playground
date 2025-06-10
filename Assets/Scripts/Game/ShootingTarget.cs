using ShootingGallery.Interfaces;
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

        [SerializeField]
        private int points = 25;

        [Tooltip("Represents which axes to rotate a target once it has been shot using Euler angles.")]
        [SerializeField]
        private Vector3 rotationAxes = new Vector3(-90.0f, 0.0f, 0.0f);

        private MeshRenderer meshRenderer;
        private bool isTargetActive = true;
        private bool isTargetInUse = false;

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

        public int Points
        {
            get => points;
            set => points = value;
        }

        public bool IsTargetInUse
        {
            get => isTargetInUse;
            set => isTargetInUse = value;
        }

        private void Awake()
        {
            meshRenderer = GetComponentInChildren<MeshRenderer>();
        }

        public void HitTarget()
        {
            if (!isTargetActive) return;

            targetHitNotify.OnTargetHit(points, targetType);
            meshRenderer.material = inactiveMaterial;
            isTargetActive = false;

            transform.rotation = Quaternion.Euler(rotationAxes.x, rotationAxes.y, rotationAxes.z);
        }

        public void ResetTarget()
        {
            if (isTargetActive) return;

            isTargetActive = true;
            isTargetInUse = false;
            targetHitNotify = null;
            meshRenderer.material = activeMaterial;
            transform.rotation = Quaternion.identity;
        }
    }
}
