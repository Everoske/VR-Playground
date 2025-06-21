using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit;

namespace ShootingGallery.XR.Weapon
{
    public class XRPistolSlider : XRBaseInteractable
    {
        [SerializeField]
        private Transform frontPoint; // Front of pistol
        [SerializeField]
        private Transform slideStopPoint; // Point where slide stops after being fired
        [SerializeField]
        private Transform backPoint; // Back of pistol (full pull back)

        [Tooltip("When active, slider follows the interactor directly")]
        [SerializeField]
        private bool directFollow = true;
        [SerializeField]
        private float followSpeed = 2.0f;

        [Tooltip("Determines how fast the slide springs back to start based on displacement")]
        [SerializeField]
        private float springTension = 1.0f;
        [Tooltip("Determines threshold for determining displacement")]
        [SerializeField]
        private float displacementThreshold = 0.011f;

        // Event called when slide pulled all the way back
        public UnityAction onPullBack;
        // Event called when slide snaps from back to front
        public UnityAction onSnapForward;

        private XRDirectInteractor currentInteractor;
        private float springBackForce = 0.0f; // Force applied to slide toward its target position
        private float displacementPercentage = 0.0f; // Used for spring back calculations and determining snap back
        private bool canInvokePullBack = true; // Ensures onPullBack isn't invoked multiple times
        private bool slideLocked = false; // Prevents the slide from moving during animation or when pistol is not held
        private bool isEmpty = false; // Parent pistol is empty
        private bool slideStopEngaged = false;

        private void Update()
        {
            if (currentInteractor != null && !slideLocked && !isEmpty)
            {
                FollowActiveInteractor();
                CheckPullBack();
                CheckReturnedToFront();
            }
            else
            {
                HandleSlideBack();
            }
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            XRDirectInteractor directInteractor = args.interactorObject as XRDirectInteractor;

            if (directInteractor != null)
            {
                currentInteractor = directInteractor;
                ResetSpringBackForce();
            }
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            XRDirectInteractor directInteractor = args.interactorObject as XRDirectInteractor;

            if (directInteractor != null && directInteractor == currentInteractor)
            {
                currentInteractor = null;
                SetSpringBackForce();
            }
        }

        public void LockSlideForAnimation()
        {
            slideLocked = true;
        }

        public void UnlockSlide()
        {
            slideLocked = false;
        }

        public void SetEmptyState(bool isEmpty)
        {
            this.isEmpty = isEmpty;
        }

        public bool SliderIsIdle()
        {
            return currentInteractor == null && transform.position == frontPoint.position;
        }

        private void FollowActiveInteractor()
        {
            Vector3 distanceToInteractor = currentInteractor.transform.position - transform.position;
            Vector3 desiredPosition = Vector3.Project(distanceToInteractor,
                (backPoint.position - frontPoint.position).normalized);

            desiredPosition = directFollow ? transform.position + desiredPosition : transform.position + desiredPosition * followSpeed * Time.deltaTime;
            transform.position = ClampedTargetPosition(desiredPosition);
        }

        private void HandleSlideBack()
        {
            if (isEmpty)
            {
                if (transform.position != backPoint.position)
                {
                    SpringToBack();
                    CheckReturnedToBack();
                }
            }
            else if (transform.position != frontPoint.position)
            {
                SpringToFront();
                CheckReturnedToFront();
            }
        }

        private void SpringToFront()
        {
            Vector3 springVelocity = (frontPoint.position - backPoint.position).normalized * Time.deltaTime * springBackForce;
            transform.position = ClampedTargetPosition(transform.position + springVelocity);
        }

        private void SpringToBack()
        {
            Vector3 springVelocity = (frontPoint.position - backPoint.position).normalized * Time.deltaTime * springTension;
            transform.position = ClampedTargetPosition(transform.position - springVelocity);
        }

        private void CheckPullBack() // Will need rework
        {
            if (transform.position == backPoint.position && canInvokePullBack)
            {
                canInvokePullBack = false;
                onPullBack?.Invoke();
            }
        }

        private void CheckReturnedToFront() // Stays mostly the same
        {
            if (transform.position != frontPoint.position) return;

            if (displacementPercentage > 1.0f - displacementThreshold) // Will need rework
            {
                onSnapForward?.Invoke();
            }

            ResetSpringBackForce();
            canInvokePullBack = true;
        }

        private void CheckReturnedToBack()
        {
            if (transform.position != backPoint.position) return;
            ResetSpringBackForce();
            canInvokePullBack = true;
            onPullBack?.Invoke();
        }

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

        private Vector3 ClampedTargetPosition(Vector3 targetPosition)
        {
            return ClampedTargetPosition(targetPosition, frontPoint.position, backPoint.position);
        }

        private void SetSpringBackForce()
        {
            if (isEmpty) return;
            displacementPercentage = (transform.position - frontPoint.position).magnitude / (backPoint.position - frontPoint.position).magnitude;
            springBackForce = springTension * displacementPercentage;
        }

        private void ResetSpringBackForce()
        {
            springBackForce = 0.0f;
            displacementPercentage = 0.0f;
        }
    }
}
