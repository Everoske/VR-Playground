using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

namespace ShootingGallery.XR
{
    public class ExternalHapticFeedbackPlayer : MonoBehaviour
    {
        [SerializeField]
        private HapticImpulsePlayer leftHapticPlayer;
        [SerializeField]
        private HapticImpulsePlayer rightHapticPlayer;

        public void SendLeftHapticImpulse(float amplitude, float duration, float frequency)
        {
            leftHapticPlayer.SendHapticImpulse(amplitude, duration, frequency);
        }

        public void SendRightHapticImpulse(float amplitude, float duration, float frequency)
        {
            rightHapticPlayer.SendHapticImpulse(amplitude, duration, frequency);
        }
    }
}