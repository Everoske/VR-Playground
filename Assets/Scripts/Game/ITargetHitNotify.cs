using ShootingGallery.Enums;

namespace ShootingGallery.Game
{
    public interface ITargetHitNotify
    {
        void OnTargetHit(TargetType type);
    }
}
