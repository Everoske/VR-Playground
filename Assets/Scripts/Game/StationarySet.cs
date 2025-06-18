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

        protected override void Start()
        {
            base.Start();
            centerPoint = targetRack.GetStartPoint() + 
                ((Vector3.Distance(targetRack.GetStartPoint(), targetRack.GetEndPoint()) / 2) * direction);
            DetermineLeadTargetPosition();
        }

        /// <summary>
        /// Move track into its position for the round and start round timer.
        /// </summary>
        protected override void ExecuteMainSequence()
        {
            base.ExecuteMainSequence();

            if (TrackInPosition())
            {
                StartCoroutine(InitiateStationarySetTimer());
            }
            else
            {
                MoveTrackIntoPosition();
            }   
        }

        /// <summary>
        /// Move track to its original position to despawn targets.
        /// </summary>
        protected override void ExecuteStopSequence()
        {
            base.ExecuteStopSequence();

            if (TrackReturned())
            {
                RemoveTargets();
            }
            else
            {
                MoveTrackToStart();
            } 
        }

        /// <summary>
        /// Starts timer for stationary target set.
        /// </summary>
        /// <returns></returns>
        private IEnumerator InitiateStationarySetTimer()
        {
            yield return new WaitForSeconds(stationarySetTime);

            if (currentState == TargetSetState.Active)
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
        /// Track reached intended position to active round.
        /// </summary>
        /// <returns></returns>
        private bool TrackInPosition()
        {
            return targetTrack.position == trackActivePosition;
        }

        /// <summary>
        /// Track returned to its original start point.
        /// </summary>
        /// <returns></returns>
        private bool TrackReturned()
        {
            return targetTrack.position == targetRack.GetStartPoint();
        }

        /// <summary>
        /// Move track toward active game position.
        /// </summary>
        private void MoveTrackIntoPosition()
        {
            TranslateTrack(trackActivePosition, direction, changePositionSpeed);
        }

        /// <summary>
        /// Move track back toward its spawn point.
        /// </summary>
        private void MoveTrackToStart()
        {
            TranslateTrack(targetRack.GetStartPoint(), -direction, changePositionSpeed);
        }
    }
}
