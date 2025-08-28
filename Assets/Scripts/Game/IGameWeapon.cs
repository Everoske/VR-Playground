using UnityEngine;

namespace ShootingGallery.Game
{
    public interface IGameWeapon
    {
        void ReturnToSpawn();
        void SetSpawnPosition(Transform spawnPoint);
        void DisableAndTerminateInteraction();
    }
}