using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace ShootingGallery.Game
{
    // Represents a single round with a series of Round Sets
    public class GalleryRound : MonoBehaviour
    {
        [SerializeField]
        private RoundSet[] sets;

        private int activeRoundSetIndex = 0;
        private bool shouldEndRound = false;

        public UnityAction onRoundReleased;

        public void InitiateGalleryRound()
        {
            if (sets == null || sets.Length == 0)
            {
                ReleaseGalleryRound();
                return;
            }

            activeRoundSetIndex = 0;
            shouldEndRound = false;
            HandleStartRoundSet();
        }

        private void OnEnable()
        {
            SubscribeToRoundSets();
        }

        private void OnDisable()
        {
            UnsubscribeFromRoundSets();
        }

        private void SubscribeToRoundSets()
        {
            foreach (RoundSet set in sets)
            {
                set.onRoundSetReleased += RoundSetComplete;
            }
        }

        private void UnsubscribeFromRoundSets()
        {
            foreach (RoundSet set in sets)
            {
                set.onRoundSetReleased -= RoundSetComplete;
            }
        }

        private void RoundSetComplete()
        {
            activeRoundSetIndex++;
            if (activeRoundSetIndex >= sets.Length)
            {
                ReleaseGalleryRound();
                return;
            }

            if (!shouldEndRound)
            {
                HandleStartRoundSet();
            }
            else
            {
                sets[activeRoundSetIndex].InitiateStopRoundSet();
            }
        }

        /// <summary>
        /// Inform GameSet that the GalleryRound has completed.
        /// </summary>
        private void ReleaseGalleryRound()
        {
            onRoundReleased?.Invoke();
        }

        private void HandleStartRoundSet()
        {
            sets[activeRoundSetIndex].InitiateRoundSet();
        }

        /// <summary>
        /// Initiate the stop procedure so the GalleryRound can be released.
        /// </summary>
        public void InitiateStopRound()
        {
            if (activeRoundSetIndex >= sets.Length)
            {
                ReleaseGalleryRound();
                return;
            }

            shouldEndRound = true;
            sets[activeRoundSetIndex].InitiateStopRoundSet();
        }

        public void AssignRoundSets()
        {
            foreach (RoundSet set in sets)
            {
                set.AssignTargetSets();
            }
        }

        public int GetTotalGalleryRoundScore()
        {
            int score = 0;
            foreach (RoundSet set in sets)
            {
                score += set.GetTotalRoundSetScore();
            }

            return score;
        }
    }
}
