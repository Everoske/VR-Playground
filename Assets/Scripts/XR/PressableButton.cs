using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

namespace ShootingGallery.XR
{
    public class PressableButton : MonoBehaviour
    {
        [Tooltip("Visual part of button that gets pressed and released")]
        [SerializeField]
        private Transform visualComponent;
        [Tooltip("Parent object of visual component")]
        [SerializeField]
        private Transform visualParent;
        [Tooltip("Position where the button is fully pressed")]
        [SerializeField]
        private Vector3 localEndPosition;

        [Tooltip("Percentage of distance where button is considered pressed")]
        [Range(0f, 1f)]
        [SerializeField]
        private float pressedThreshold = 0.15f;

        [Tooltip("Time for the button to return to its default position")]
        [SerializeField]
        private float springBackTime = 0.5f;
        [Tooltip("Reset time for triggering onPressed after the button has been released")]
        [SerializeField]
        private float deadtime = 0.25f;

        private Vector3 localDefaultPosition;
        private Vector3 pathNormal;
        private Vector3 movement;

        private XRSimpleInteractable interactable;
        private Transform pokeTransform;

        private bool isInteracting = false;
        private bool isPressed = false;
        private bool canTriggerPressed = true;

        private float springCounter = 0.0f;

        public bool IsPressed => isPressed;

        private void Awake()
        {
            interactable = GetComponent<XRSimpleInteractable>();
        }

        private void OnEnable()
        {
            interactable.hoverEntered.AddListener(HoverEntered);
            interactable.hoverExited.AddListener(HoverExited);
        }

        private void OnDisable()
        {
            interactable.hoverEntered.RemoveAllListeners();
            interactable.hoverExited.RemoveAllListeners();
        }

        private void Start()
        {
            localDefaultPosition = visualComponent.localPosition;
            pathNormal = (localEndPosition - localDefaultPosition).normalized;
        }

        private void Update()
        {
            if (isInteracting)
            {
                MoveVisual();
            }
            else if (visualComponent.localPosition != localDefaultPosition)
            {
                SpringBack();
            }

            CheckPressed();
        }

        /// <summary>
        /// Determines how much the visual component should move
        /// based on the distance from a valid poke interactor
        /// that is actively hovering over the XR Interactable
        /// </summary>
        /// <param name="hover">Hover event</param>
        private void HoverEntered(BaseInteractionEventArgs hover)
        {
            if (hover.interactorObject is XRPokeInteractor)
            {
                XRPokeInteractor pokeInteractor = (XRPokeInteractor)hover.interactorObject;
                isInteracting = true;
                pokeTransform = pokeInteractor.transform;
                movement = visualComponent.position - pokeTransform.position;
            }
        }

        /// <summary>
        /// Returns the button to its default state
        /// </summary>
        /// <param name="hover">Hover event</param>
        private void HoverExited(BaseInteractionEventArgs hover)
        {
            isInteracting = false;
        }

        /// <summary>
        /// Moves the visual component between its default and end positions
        /// based on the distance from a poke interactor
        /// </summary>
        private void MoveVisual()
        {
            Vector3 localTargetPosition = visualComponent.InverseTransformPoint(pokeTransform.position + movement);
            localTargetPosition = Vector3.Project(localTargetPosition, pathNormal);
            visualComponent.position = ClampedTargetPosition(visualComponent.TransformPoint(localTargetPosition));
        }

        /// <summary>
        /// Clamps a world position for the visual component between the
        /// default and end positions
        /// </summary>
        /// <param name="targetPosition">World position to move visual to</param>
        /// <returns>Clamped world position for visual</returns>
        private Vector3 ClampedTargetPosition(Vector3 targetPosition)
        {
            Vector3 defaultPosition = visualParent.TransformPoint(localDefaultPosition);
            Vector3 endPosition = visualParent.TransformPoint(localEndPosition);

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
        /// Returns the visual component to its default position over time
        /// </summary>
        private void SpringBack()
        {
            springCounter += Time.deltaTime;
            visualComponent.localPosition =
                Vector3.Lerp(visualComponent.localPosition, localDefaultPosition, springCounter / springBackTime);

            if (visualComponent.localPosition == localDefaultPosition)
            {
                springCounter = 0;
            }
        }

        /// <summary>
        /// Determines if the button is pressed or released based on the 
        /// distance of the visual component to the end position. Invokes
        /// onPressed and onReleased when applicable
        /// </summary>
        private void CheckPressed()
        {
            float percentDistance =
                (visualComponent.localPosition - localEndPosition).magnitude / (localDefaultPosition - localEndPosition).magnitude;

            if (percentDistance <= pressedThreshold)
            {
                if (canTriggerPressed)
                {
                    interactable.selectEntered?.Invoke(new SelectEnterEventArgs());
                }
                canTriggerPressed = false;
                isPressed = true;
            }
            else
            {
                if (isPressed)
                {
                    interactable.selectExited?.Invoke(new SelectExitEventArgs());
                    StartCoroutine(ProcessDeadtime());
                }
                isPressed = false;
            }
        }

        /// <summary>
        /// Resets the ability for the button to trigger its onPressed
        /// event after a certain amount of time has passed since it
        /// was released
        /// </summary>
        /// <returns>Coroutine</returns>
        private IEnumerator ProcessDeadtime()
        {
            yield return new WaitForSeconds(deadtime);
            if (!isPressed)
            {
                canTriggerPressed = true;
            }
        }
    }
}
