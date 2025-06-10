using Unity.VisualScripting;
using UnityEngine;

namespace ShootingGallery.Game
{
    // Spawn Outside View - Done in Target Set
    // Move to Center
    // Targets stay centered until condition met
    // Move targets outside view
    // Despawn targets

    // TODO: Remove targets outside of the visible area to the player
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

        }

        public override void InitiateTargetSet()
        {
            base.InitiateTargetSet();
            if (shootingTargets.Count == 0) return;

            
        }

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

        private bool LeadTargetInPosition()
        {
            if (shootingTargets.Count == 0) return false;
            return shootingTargets[0].transform.position == leadTargetPosition;
        }

        private bool LeadTargetReturned()
        {
            if (shootingTargets.Count == 0) return false;
            return shootingTargets[0].transform.position == startPoint.position;
        }
    }
}
