using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace ShootingGallery.XR.Weapon
{
    /// <summary>
    /// Represents the bolt component of a bolt-action rifle.
    /// </summary>
    public class XRBolt : XRBaseInteractable
    {
        [SerializeField]
        private float pulledUpRotation = -90.0f;
        [SerializeField]
        private Transform attachTransform;

        [SerializeField]
        private Transform defaultPoint;
        [SerializeField]
        private Transform outPoint;
        [SerializeField]
        private Transform rotationOrigin;

        private Vector2 localAttachOrigin;
        private XRDirectInteractor currentInteractor;

        private void Start()
        {
            Vector3 attachRelative = rotationOrigin.InverseTransformPoint(attachTransform.position);
            localAttachOrigin = new Vector2(
                attachRelative.z,
                attachRelative.y
                );
        }

        private void Update()
        {
            if (currentInteractor != null)
            {
                RotateWithInteractor();
                MoveWithInteractor();
            }
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            XRDirectInteractor directInteractor = args.interactorObject as XRDirectInteractor;

            if (directInteractor != null)
            {
                currentInteractor = directInteractor;
            }
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            XRDirectInteractor directInteractor = args.interactorObject as XRDirectInteractor;

            if (directInteractor != null && directInteractor == currentInteractor)
            {
                currentInteractor = null;
            }
        }

        /// <summary>
        /// Rotates the bolt-handle up and down relative to the
        /// active interactor.
        /// </summary>
        private void RotateWithInteractor()
        {
            if (transform.position != rotationOrigin.position) return;

            Vector3 interactorRelative = rotationOrigin.InverseTransformPoint(currentInteractor.attachTransform.position);

            Vector2 interactorPosZY = new Vector2(
                interactorRelative.z,
                interactorRelative.y
                );

            float desiredAngle = Vector2.SignedAngle(interactorPosZY, localAttachOrigin);
            desiredAngle = Mathf.Clamp(desiredAngle, pulledUpRotation, 0.0f);
            transform.localRotation = Quaternion.Euler(new Vector3(desiredAngle, 0.0f, 0.0f));
        }

        /// <summary>
        /// Moves the bolt with the active interactor when the bolt
        /// handle is pulled all the way up. 
        /// </summary>
        private void MoveWithInteractor()
        {
            if (!BoltPulledUp()) return;
            Vector3 distanceToInteractor = currentInteractor.attachTransform.position - transform.position;
            Vector3 desiredPosition = Vector3.Project(distanceToInteractor,
                (outPoint.position - defaultPoint.position).normalized);

            desiredPosition = transform.position + desiredPosition;
            transform.position = ClampedTargetPosition(desiredPosition, defaultPoint.position, outPoint.position);
        }

        /// <summary>
        /// Determines whether the bolt is pulled all the way up.
        /// </summary>
        /// <returns></returns>
        private bool BoltPulledUp()
        {
            return transform.localEulerAngles.x == 360.0f - Mathf.Abs(pulledUpRotation);
        }

        /// <summary>
        /// Clamp the slide's movements between two 3D positions.
        /// </summary>
        /// <param name="targetPosition">Intended target position.</param>
        /// <param name="defaultPosition">Starting position of track to follow.</param>
        /// <param name="endPosition">Ending position of track to follow.</param>
        /// <returns>Adjusted target position.</returns>
        private Vector3 ClampedTargetPosition(Vector3 targetPosition, Vector3 defaultPosition, Vector3 endPosition)
        {
            float x = Mathf.Clamp(
                targetPosition.x,
                Mathf.Min(defaultPosition.x, endPosition.x),
                Mathf.Max(defaultPosition.x, endPosition.x)
                );

            float y = Mathf.Clamp(
                targetPosition.y,
                Mathf.Min(defaultPosition.y, endPosition.y),
                Mathf.Max(defaultPosition.y, endPosition.y)
                );

            float z = Mathf.Clamp(
                targetPosition.z,
                Mathf.Min(defaultPosition.z, endPosition.z),
                Mathf.Max(defaultPosition.z, endPosition.z)
                );

            return new Vector3(x, y, z);
        }
    }
}
