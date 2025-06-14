using ShootingGallery.Game;

namespace ShootingGallery.Interfaces
{
    public interface ITargetHitNotify
    {
        void OnTargetHit(TargetType type);
    }
}
