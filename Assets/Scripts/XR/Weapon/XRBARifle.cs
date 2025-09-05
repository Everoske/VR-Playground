using ShootingGallery.Settings;
using ShootingGallery.UI;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace ShootingGallery.XR.Weapon
{
    public class XRBARifle : XRGrabInteractable
    {
        [SerializeField]
        private XRBolt bolt;

        [SerializeField]
        private XRRifleChamber chamber;

        [SerializeField]
        private AmmoCounterUI ammoCounterUI;

        [Tooltip("Locks the bolt when rifle not held.")]
        [SerializeField]
        private bool lockBoltWhenNotHeld = true;

        [Header("Haptic Feedback Settings")]
        [SerializeField]
        private ExternalHapticFeedbackPlayer hapticFeedbackPlayer;
        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float recoilAmplitude = 0.5f;
        [SerializeField]
        private float recoilDuration = 0.25f;
        [SerializeField]
        private float recoilFrequency = 0.0f;

        private RifleFireState fireState = RifleFireState.Empty;

        protected override void OnEnable()
        {
            base.OnEnable();
            bolt.onBoltPulledUp += OnBoltPulledUp;
            bolt.onBoltPulledBack += OnBoltPulledBack;
            bolt.onBoltUnobstruct += OnBoltUnobstruct;
            bolt.onBoltObstruct += OnBoltObstruct;
            bolt.onBoltPushedIn += OnBoltPushedIn;

            chamber.onLoadSequenceStart += OnAmmoLoadStart;
            chamber.onLoadSequenceEnd += OnAmmoLoadEnd;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            bolt.onBoltPulledUp -= OnBoltPulledUp;
            bolt.onBoltPulledBack -= OnBoltPulledBack;
            bolt.onBoltUnobstruct -= OnBoltUnobstruct;
            bolt.onBoltObstruct -= OnBoltObstruct;
            bolt.onBoltPushedIn -= OnBoltPushedIn;

            chamber.onLoadSequenceStart -= OnAmmoLoadStart;
            chamber.onLoadSequenceEnd -= OnAmmoLoadEnd;
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);

            if (lockBoltWhenNotHeld)
            {
                bolt.SetLockBolt(false);
            }
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);

            if (lockBoltWhenNotHeld)
            {
                bolt.SetLockBolt(true);
            }
        }

        /// <summary>
        /// Determines if gun can fire.
        /// </summary>
        /// <returns></returns>
        private bool CanFire()
        {
            return fireState == RifleFireState.LiveRoundInBarrel && bolt.IsBoltClosed();
        }

        private void OnBoltPulledUp()
        {
            // Play sound
        }

        private void OnBoltPulledBack()
        {
            // Eject round if in barrel
            switch (fireState)
            {
                case RifleFireState.LiveRoundInBarrel:
                    // EjectLiveRound();
                    break;
                case RifleFireState.CasingInBarrel:
                    // EjectCasing();
                    break;
            }

            // Play sound
        }

        
        private void OnBoltUnobstruct()
        {
            chamber.SetChamberClosed(false);
        }

        private void OnBoltObstruct()
        {
            chamber.SetChamberClosed(true);
        }

        private void OnBoltPushedIn()
        {
            if (fireState == RifleFireState.Empty && chamber.HasAmmo())
            {
                chamber.ReduceAmmoCount();
                fireState = RifleFireState.LiveRoundInBarrel;
            }
        }

        private void OnAmmoLoadStart()
        {
            bolt.SetLockBolt(true);
        }

        private void OnAmmoLoadEnd()
        {
            bolt.SetLockBolt(false);
        }
    }

    public enum RifleFireState
    {
        Empty,
        CasingInBarrel,
        LiveRoundInBarrel
    }
}
