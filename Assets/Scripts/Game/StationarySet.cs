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

        protected override void Start()
        {
            base.Start();
            centerPoint = startPoint.position + 
                ((Vector3.Distance(startPoint.position, endPoint.position) / 2) * direction);
        }

        public override void InitiateTargetSet()
        {
            base.InitiateTargetSet();
            if (shootingTargets.Count == 0) return;

            
        }

    }
}
