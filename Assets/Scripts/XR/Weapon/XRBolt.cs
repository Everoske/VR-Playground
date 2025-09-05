using UnityEngine;
using UnityEngine.Events;
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

        [Tooltip("Threshold of distance pulled back that bolt is considered obstructing the rifle chamber.")]
        [SerializeField]
        private float obstructPercentage = 0.85f;

        [SerializeField]
        private InteractionLayerMask defaultInteractionLayerMask;

        private Vector2 localAttachOrigin;
        private XRDirectInteractor currentInteractor;

        private bool lockBolt = true;

        public UnityAction onBoltPulledUp;
        public UnityAction onBoltPulledBack;
        public UnityAction onBoltUnobstruct;
        public UnityAction onBoltObstruct;
        public UnityAction onBoltPushedIn;

        private void Start()
        {
            Vector3 attachRelative = rotationOrigin.InverseTransformPoint(attachTransform.position);
            localAttachOrigin = new Vector2(
                attachRelative.z,
                attachRelative.y
                );

            SetLockBolt(lockBolt);
        }

        private void Update()
        {
            if (currentInteractor != null && !lockBolt)
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
        /// Lock the bolt from interaction.
        /// </summary>
        /// <param name="lockBolt"></param>
        public void SetLockBolt(bool lockBolt)
        {
            this.lockBolt = lockBolt;

            if (lockBolt)
            {
                interactionLayers = InteractionLayerMask.GetMask("Nothing");
            }
            else
            {
                interactionLayers = defaultInteractionLayerMask;
            }
        }

        /// <summary>
        /// Determines if the bolt is pushed in with the handle pulled all the
        /// way down. 
        /// </summary>
        /// <returns></returns>
        public bool IsBoltClosed()
        {
            return transform.localEulerAngles.x == 0.0f;
        }

        /// <summary>
        /// Rotates the bolt-handle up and down relative to the
        /// active interactor.
        /// </summary>
        private void RotateWithInteractor()
        {
            if (!IsBoltPushedIn()) return;

            bool wasBoltPulledUpLastFrame = IsBoltPulledUp();

            Vector3 interactorRelative = rotationOrigin.InverseTransformPoint(currentInteractor.attachTransform.position);

            Vector2 interactorPosZY = new Vector2(
                interactorRelative.z,
                interactorRelative.y
                );

            float desiredAngle = Vector2.SignedAngle(interactorPosZY, localAttachOrigin);
            desiredAngle = Mathf.Clamp(desiredAngle, pulledUpRotation, 0.0f);
            transform.localRotation = Quaternion.Euler(new Vector3(desiredAngle, 0.0f, 0.0f));

            if (IsBoltPulledUp() && !wasBoltPulledUpLastFrame)
            {
                onBoltPulledUp?.Invoke();
            }
        }

        /// <summary>
        /// Moves the bolt with the active interactor when the bolt
        /// handle is pulled all the way up. 
        /// </summary>
        private void MoveWithInteractor()
        {
            if (!IsBoltPulledUp()) return;

            bool wasBoltPulledBackLastFrame = IsBoltPulledBack();
            bool wasBoltObstructingLastFrame = IsBoltObstructing();
            bool wasBoltPushedInLastFrame = IsBoltPushedIn();

            Vector3 distanceToInteractor = currentInteractor.attachTransform.position - transform.position;
            Vector3 desiredPosition = Vector3.Project(distanceToInteractor,
                (outPoint.position - defaultPoint.position).normalized);

            desiredPosition = transform.position + desiredPosition;
            transform.position = ClampedTargetPosition(desiredPosition, defaultPoint.position, outPoint.position);

            DeterminePulledBackThisFrame(wasBoltPulledBackLastFrame);
            DetermineObstructionChange(wasBoltObstructingLastFrame);
            DeterminePushedInThisFrame(wasBoltPushedInLastFrame);
        }

        /// <summary>
        /// Determines whether the bolt is pulled all the way up.
        /// </summary>
        /// <returns></returns>
        private bool IsBoltPulledUp()
        {
            return transform.localEulerAngles.x == 360.0f - Mathf.Abs(pulledUpRotation);
        }

        /// <summary>
        /// Determines whether the bolt is pulled all the way back.
        /// </summary>
        /// <returns></returns>
        private bool IsBoltPulledBack()
        {
            return transform.position == outPoint.position;
        }

        /// <summary>
        /// Determines whether the bolt is pushed all the way in.
        /// </summary>
        /// <returns></returns>
        private bool IsBoltPushedIn()
        {
            return transform.position == defaultPoint.position;
        }

        /// <summary>
        /// Determines whether the bolt is obstructing the rifle chamber.
        /// </summary>
        /// <returns></returns>
        private bool IsBoltObstructing()
        {
            float percentDistance = Vector3.Distance(transform.position, outPoint.position) / 
                Vector3.Distance(defaultPoint.position, outPoint.position);

            return percentDistance < obstructPercentage;
        }

        /// <summary>
        /// Determines if the bolt was pulled back this frame and calls the
        /// callback method.
        /// </summary>
        /// <param name="wasBoltPulledBackLastFrame"></param>
        private void DeterminePulledBackThisFrame(bool wasBoltPulledBackLastFrame)
        {
            if (IsBoltPulledBack() && !wasBoltPulledBackLastFrame)
            {
                onBoltPulledBack?.Invoke();
            }
        }

        /// <summary>
        /// Determines if the bolt obstructs or unobstructs the rifle chamber
        /// this frame and calls the appropriate callbacks.
        /// </summary>
        /// <param name="wasBoltObstructingLastFrame"></param>
        private void DetermineObstructionChange(bool wasBoltObstructingLastFrame)
        {
            if (IsBoltObstructing() && !wasBoltObstructingLastFrame)
            {
                onBoltObstruct?.Invoke();
            }

            if (!IsBoltObstructing() && wasBoltObstructingLastFrame)
            {
                onBoltUnobstruct?.Invoke();
            }
        }

        /// <summary>
        /// Determines if bolt was pushed in this frame and calls the
        /// callback if it was.
        /// </summary>
        /// <param name="wasBoltPushedInLastFrame"></param>
        private void DeterminePushedInThisFrame(bool wasBoltPushedInLastFrame)
        {
            if (IsBoltPushedIn() && !wasBoltPushedInLastFrame)
            {
                onBoltPushedIn?.Invoke();
            }
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
