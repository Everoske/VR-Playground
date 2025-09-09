using UnityEngine;

namespace ShootingGallery.XR
{
    // TODO: Implement null service pattern.
    public class ExternalHapticFeedbackPlayerLocator
    {
        private static ExternalHapticFeedbackPlayer hapticFeedbackPlayer;

        public static void Provide(ExternalHapticFeedbackPlayer player)
        {
            hapticFeedbackPlayer = player;
        }

        public static ExternalHapticFeedbackPlayer GetHapticFeedbackPlayer() => hapticFeedbackPlayer;
    }
}