using ShootingGallery.Game;

namespace ShootingGallery.Game
{
    public interface ITargetHitNotify
    {
        void OnTargetHit(TargetType type);
    }
}
