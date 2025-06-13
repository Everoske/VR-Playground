using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace ShootingGallery.Game
{
    /// <summary>
    /// Represents a stationary target set.
    /// </summary>
    public class StationarySet : TargetSet
    {
        [Tooltip("Speed at which the Stationary Targets move into and out of position.")]
        [SerializeField]
        private float changePositionSpeed = 2.0f;

        [Tooltip("Time until stationary set automatically stops.")]
        [SerializeField]
        private float stationarySetTime = 10.0f;

        private Vector3 centerPoint;
        private Vector3 leadTargetPosition;
        private bool setActive = false;
        private bool setTimerActive = false;
        private bool returnToStart = true;

        private void Awake()
        {
            setType = SetType.Stationary;
        }
        protected override void Start()
        {
            base.Start();
            centerPoint = startPoint.position + 
                ((Vector3.Distance(startPoint.position, endPoint.position) / 2) * direction);
            DetermineLeadTargetPosition();
        }

        protected override void Update()
        {
            base.Update();
            ProcessTargetPositions();
        }

        public override void InitiateTargetSet()
        {
            base.InitiateTargetSet();
            if (shootingTargets.Length == 0) return;
            setActive = true;
            returnToStart = false;
        }

        public override void StopTargetSet()
        {
            base.StopTargetSet();
            setTimerActive = false;
            returnToStart = true;
        }

        /// <summary>
        /// Starts timer for stationary target set.
        /// </summary>
        /// <returns></returns>
        private IEnumerator InitiateStationarySetTimer()
        {
            setTimerActive = true;
            yield return new WaitForSeconds(stationarySetTime);

            if (setTimerActive)
            {
                StopTargetSet();
            }
        }

        /// <summary>
        /// Determines where the leading target should be positioned.
        /// </summary>
        private void DetermineLeadTargetPosition()
        {
            float leadOffset = 0.0f;

            if (setOrder.Length % 2 == 0)
            {
                leadOffset = (setOrder.Length / 2 - 1) * distanceBetweenTargets + distanceBetweenTargets / 2;
            }
            else
            {
                leadOffset = (setOrder.Length - 1) / 2 * distanceBetweenTargets;
            }

            leadTargetPosition = centerPoint + leadOffset * direction;
        }

        /// <summary>
        /// Determines where the targets should move.
        /// </summary>
        private void ProcessTargetPositions()
        {
            if (!setActive) return;

            if (!returnToStart)
            {
                MoveTargetsIntoPosition();
            }
            else 
            {
                MoveTargetsToStart();
            }
        }

        /// <summary>
        /// Determines if the lead target is in the desired position for active gameplay.
        /// </summary>
        /// <returns>True if in desired position.</returns>
        private bool LeadTargetInPosition()
        {
            if (shootingTargets.Length == 0) return false;
            return shootingTargets[0].transform.position == leadTargetPosition;
        }

        /// <summary>
        /// Deterimes if the lead target has returned to its original spawn point.
        /// </summary>
        /// <returns>True if returned to spawn point.</returns>
        private bool LeadTargetReturned()
        {
            if (shootingTargets.Length == 0) return false;
            return shootingTargets[0].transform.position == startPoint.position;
        }

        /// <summary>
        /// Moves targets into the desired position for active gameplay.
        /// </summary>
        private void MoveTargetsIntoPosition()
        {
            if (LeadTargetInPosition())
            {
                StartCoroutine(InitiateStationarySetTimer());
                return;
            }
            TranslateTargets(leadTargetPosition, direction);
        }

        /// <summary>
        /// Returns targets to their spawn points.
        /// </summary>
        private void MoveTargetsToStart()
        {
            if (LeadTargetReturned())
            {
                RemoveTargets();
                setActive = false;
                return;
            }
            TranslateTargets(startPoint.position, -direction);
        }

        /// <summary>
        /// Moves targets toward a specified position.
        /// </summary>
        /// <param name="targetPosition">Desired position.</param>
        /// <param name="currentDirection">Direction toward desired position.</param>
        private void TranslateTargets(Vector3 targetPosition, Vector3 currentDirection)
        {
            float speed = changePositionSpeed * Time.deltaTime;
            if (speed >= (targetPosition - shootingTargets[0].transform.position).magnitude)
            {
                speed = (targetPosition - shootingTargets[0].transform.position).magnitude;
            }

            for (int i = 0; i < shootingTargets.Length; i++)
            {
                shootingTargets[i].transform.Translate(currentDirection * speed);
            }
        }
    }
}
