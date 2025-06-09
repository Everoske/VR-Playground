using ShootingGallery.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ShootingGallery.Game
{
    public class TargetSetManager : MonoBehaviour, ITargetHitNotify
    {
        [SerializeField]
        private Transform startPoint;
        [SerializeField]
        private Transform endPoint;

        private int activeSetIndex;
        private int targetsThisRound;

        public UnityAction<int> onTargetHit;
        public UnityAction onRoundComplete; // Needs examination

        public void OnTargetHit(int points, TargetType type)
        {
            if (type == TargetType.Normal)
            {
                targetsThisRound--;
                // points = points * targetSets[activeSetIndex].GetSetMultiplier();
            }

            onTargetHit?.Invoke(points);
        }
    }
}
