using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

namespace ShootingGallery.XR
{
    /// <summary>
    /// Plays haptic feedback impulses to the player's controllers based on input from
    /// in-game components.
    /// </summary>
    public class ExternalHapticFeedbackPlayer : MonoBehaviour
    {
        [SerializeField]
        private HapticImpulsePlayer leftHapticPlayer;
        [SerializeField]
        private HapticImpulsePlayer rightHapticPlayer;

        private void Awake()
        {
            ExternalHapticFeedbackPlayerLocator.Provide(this);
        }

        /// <summary>
        /// Send haptic feedback to the player's left controller.
        /// </summary>
        /// <param name="amplitude">Desired motor amplitude.</param>
        /// <param name="duration">Desired duration of impulse.</param>
        /// <param name="frequency">The desired frequency of the impusle in Hz.</param>
        public void SendLeftHapticImpulse(float amplitude, float duration, float frequency)
        {
            leftHapticPlayer.SendHapticImpulse(amplitude, duration, frequency);
        }

        /// <summary>
        /// Send haptic feedback to the player's right controller.
        /// </summary>
        /// <param name="amplitude">Desired motor amplitude.</param>
        /// <param name="duration">Desired duration of impulse.</param>
        /// <param name="frequency">The desired frequency of the impusle in Hz.</param>
        public void SendRightHapticImpulse(float amplitude, float duration, float frequency)
        {
            rightHapticPlayer.SendHapticImpulse(amplitude, duration, frequency);
        }
    }
}