using UnityEngine;

namespace ShootingGallery.Game
{
    public interface IGameWeapon
    {
        void HandleOutOfBounds();
        void DetachFromPlayer();
    }
}