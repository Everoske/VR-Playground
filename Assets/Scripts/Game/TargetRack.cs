using UnityEngine;

namespace ShootingGallery.Game
{
    public class TargetRack : MonoBehaviour
    {
        [SerializeField]
        private Transform startPoint;
        [SerializeField]
        private Transform endPoint;

        public Vector3 GetStartPoint()
        {
            if (startPoint == null)
            {
                throw new System.Exception("TargetRack: Start Point requested is null.");
            }

            return startPoint.position;
        }

        public Vector3 GetEndPoint()
        {
            if (endPoint == null)
            {
                throw new System.Exception("TargetRack: End Point requested is null.");
            }

            return endPoint.position;
        }

        public Vector3 GetRackDirection()
        {
            return (endPoint.position - startPoint.position).normalized;
        }
    }
}
