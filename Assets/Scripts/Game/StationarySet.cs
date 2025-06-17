using System.Collections;
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
        private Vector3 trackActivePosition;
        private bool setActive = false;
        private bool setTimerActive = false;
        private bool returnToStart = true;

        protected override void Start()
        {
            base.Start();
            centerPoint = targetRack.GetStartPoint() + 
                ((Vector3.Distance(targetRack.GetStartPoint(), targetRack.GetEndPoint()) / 2) * direction);
            DetermineLeadTargetPosition();
        }

        private void Update()
        {
            
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
            if (!IsSetActive()) return;

            setTimerActive = false;
            returnToStart = true;
        }

        protected override bool IsSetActive()
        {
            return setActive;
        }

        protected override void ExecuteMainSequence()
        {
            base.ExecuteMainSequence();
            ProcessTrackPosition();
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

            trackActivePosition = centerPoint + leadOffset * direction;
        }

        /// <summary>
        /// Determines where the targets should move.
        /// </summary>
        private void ProcessTrackPosition()
        {
            if (!setActive) return;

            if (!returnToStart)
            {
                MoveTrackIntoPosition();
            }
            else 
            {
                MoveTrackToStart();
            }
        }

        private bool TrackInPosition()
        {
            return targetTrack.position == trackActivePosition;
        }

        private bool TrackReturned()
        {
            return targetTrack.position == targetRack.GetStartPoint();
        }

        private void MoveTrackIntoPosition()
        {
            if (TrackInPosition())
            {
                StartCoroutine(InitiateStationarySetTimer());
                return;
            }
            TranslateTrack(trackActivePosition, direction, changePositionSpeed);
        }

        private void MoveTrackToStart()
        {
            if (TrackReturned())
            {
                RemoveTargets();
                setActive = false;
                return;
            }
            TranslateTrack(targetRack.GetStartPoint(), -direction, changePositionSpeed);
        }
    }
}
