using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace ShootingGallery.XR.Weapon
{
    /// <summary>
    /// Represents the slider component of a semi-automatic pistol.
    /// </summary>
    public class XRPistolSlider : XRBaseInteractable
    {
        [Tooltip("Represents the default position for the slide.")]
        [SerializeField]
        private Transform frontPoint;
        [Tooltip("Represents the position where the slide locks after firing until empty.")]
        [SerializeField]
        private Transform slideStopPoint;
        [Tooltip("Represents the farthest position the slide can be pulled back.")]
        [SerializeField]
        private Transform backPoint; 

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

        public UnityAction onPullBack;
        public UnityAction onSnapForward;

        private XRDirectInteractor currentInteractor;
        private float springBackForce = 0.0f; 
        private float displacementPercentage = 0.0f; 
        private bool canInvokePullBack = true; 
        private bool slideLocked = false; 
        private bool slideStopEngaged = false;

        private void Update()
        {
            if (currentInteractor != null && !slideLocked)
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
                DisengageSlideStop();
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

        /// <summary>
        /// Prevent slide from moving when an animation is playing.
        /// </summary>
        public void LockSlideForAnimation()
        {
            slideLocked = true;
        }

        /// <summary>
        /// Allow slide to move following animation completion.
        /// </summary>
        public void UnlockSlide()
        {
            slideLocked = false;
        }

        /// <summary>
        /// Engage slide stop.
        /// </summary>
        public void EngageSlideStop()
        {
            slideStopEngaged = true;
        }

        /// <summary>
        /// Determine if slide is not being interacted with and in its default position.
        /// </summary>
        /// <returns></returns>
        public bool SliderIsIdle()
        {
            return currentInteractor == null && transform.position == frontPoint.position;
        }

        /// <summary>
        /// Disengage slide stop.
        /// </summary>
        private void DisengageSlideStop()
        {
            slideStopEngaged = false;
        }

        /// <summary>
        /// Have the slide follow the player's hand movement. 
        /// </summary>
        private void FollowActiveInteractor()
        {
            Vector3 distanceToInteractor = currentInteractor.transform.position - transform.position;
            Vector3 desiredPosition = Vector3.Project(distanceToInteractor,
                (backPoint.position - frontPoint.position).normalized);

            desiredPosition = directFollow ? transform.position + desiredPosition : transform.position + desiredPosition * followSpeed * Time.deltaTime;
            transform.position = ClampedTargetPosition(desiredPosition);
        }

        /// <summary>
        /// Handle sliding the pistol to its default position or the slide stop position if
        /// the slide stop is engaged.
        /// </summary>
        private void HandleSlideBack()
        {
            if (slideStopEngaged)
            {
                if (transform.position != slideStopPoint.position)
                {
                    SpringToSlideStop();
                    CheckReachedSlideStop();
                }
            }
            else if (transform.position != frontPoint.position)
            {
                SpringToFront();
                CheckReturnedToFront();
            }
        }

        /// <summary>
        /// Move the slide back toward its default position.
        /// </summary>
        private void SpringToFront()
        {
            Vector3 springVelocity = (frontPoint.position - backPoint.position).normalized * Time.deltaTime * springBackForce;
            transform.position = ClampedTargetPosition(transform.position + springVelocity);
        }

        /// <summary>
        /// Move the slide toward the slide stop position.
        /// </summary>
        private void SpringToSlideStop()
        {
            Vector3 springVelocity = (frontPoint.position - slideStopPoint.position).normalized * Time.deltaTime * springTension;
            transform.position = ClampedTargetPosition(transform.position - springVelocity,
                frontPoint.position, slideStopPoint.position);
        }

        /// <summary>
        /// Determine if slide has been fully pulled back by player.
        /// </summary>
        private void CheckPullBack() 
        {
            if (transform.position == backPoint.position && canInvokePullBack)
            {
                canInvokePullBack = false;
                onPullBack?.Invoke();
            }
        }

        /// <summary>
        /// Check if slide returned to its default position. Check if the slide 
        /// was released far back enough to load a round into the pistol's chamber.
        /// </summary>
        private void CheckReturnedToFront() 
        {
            if (transform.position != frontPoint.position) return;

            if (displacementPercentage > 1.0f - displacementThreshold) 
            {
                onSnapForward?.Invoke();
            }

            ResetSpringBackForce();
            canInvokePullBack = true;
        }

        /// <summary>
        /// Check if slide reached the slide stop position.
        /// </summary>
        private void CheckReachedSlideStop()
        {
            if (transform.position != slideStopPoint.position) return;
            ResetSpringBackForce();
            canInvokePullBack = true;
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

        /// <summary>
        /// Clamp the slide's movements between the start point and back point positions.
        /// </summary>
        /// <param name="targetPosition">Intended target position.</param>
        /// <returns>Adjusted target position.</returns>
        private Vector3 ClampedTargetPosition(Vector3 targetPosition)
        {
            return ClampedTargetPosition(targetPosition, frontPoint.position, backPoint.position);
        }

        /// <summary>
        /// Set the force applied to the slide when released based on how far it is pulled back
        /// from its default position.
        /// </summary>
        private void SetSpringBackForce()
        {
            displacementPercentage = (transform.position - frontPoint.position).magnitude / (backPoint.position - frontPoint.position).magnitude;
            springBackForce = springTension * displacementPercentage;
        }

        /// <summary>
        /// Reset the spring force and displacement values.
        /// </summary>
        private void ResetSpringBackForce()
        {
            springBackForce = 0.0f;
            displacementPercentage = 0.0f;
        }
    }
}
