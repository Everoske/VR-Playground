using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

namespace ShootingGallery.Integrity
{
    /// <summary>
    /// Represents a restricted area that the player should not be allowed in. Teleports player to a 
    /// non-restricted destination.
    /// </summary>
    public class PlayerRestrictedVolume : MonoBehaviour
    {
        [SerializeField]
        private TeleportationProvider teleportProvider;
        [SerializeField]
        private Transform destination;
        [SerializeField]
        private MatchOrientation matchOrientation;

        private void OnTriggerEnter(Collider other)
        {
            TeleportPlayer();
        }

        /// <summary>
        /// Teleport player to non-restricted area.
        /// </summary>
        private void TeleportPlayer()
        {
            TeleportRequest request = new TeleportRequest();

            request.matchOrientation = matchOrientation;
            request.destinationPosition = destination.position;
            request.destinationRotation = destination.rotation;

            teleportProvider.QueueTeleportRequest(request);
        }
    }
}
