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

        [SerializeField]
        private float timeBeforeRound = 5.0f; // Used by GameSet

        private int activeRoundSetIndex = 0;
        private int roundScore = 0;

        public UnityAction<int> onRoundComplete;

        public void InitiateGalleryRound()
        {
            roundScore = 0;
            activeRoundSetIndex = 0;
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
                set.onRoundSetComplete += RoundSetComplete;
            }
        }

        private void UnsubscribeFromRoundSets()
        {
            foreach (RoundSet set in sets)
            {
                set.onRoundSetComplete -= RoundSetComplete;
            }
        }

        private void RoundSetComplete(int setScore)
        {
            roundScore += setScore;
            activeRoundSetIndex++;

            if (activeRoundSetIndex >= sets.Length)
            {
                onRoundComplete?.Invoke(roundScore);
                return;
            }

            HandleStartRoundSet();
        }

        private void HandleStartRoundSet()
        {
            if (sets[activeRoundSetIndex].TimeBeforeSet > 0.0f)
            {
                StartCoroutine(StartSetAfterDelay());
            }
            else
            {
                sets[activeRoundSetIndex].InitiateRoundSet();
            }
        }

        private IEnumerator StartSetAfterDelay()
        {
            yield return new WaitForSeconds(sets[activeRoundSetIndex].TimeBeforeSet);
            sets[activeRoundSetIndex].InitiateRoundSet();
        }

        public void StopRound()
        {
            if (activeRoundSetIndex >= sets.Length) return;
            sets[activeRoundSetIndex].StopRoundSet();
        }
    }
}
