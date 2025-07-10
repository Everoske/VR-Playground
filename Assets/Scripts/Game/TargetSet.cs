using UnityEngine;
using UnityEngine.Events;

using ShootingGallery.Enums;

namespace ShootingGallery.Game
{
    /// <summary>
    /// Represents a single set of similar targets for the player to hit.
    /// </summary>
    public class TargetSet : MonoBehaviour, ITargetHitNotify
    {
        [SerializeField]
        private TargetPool targetPool;

        [Tooltip("The number target types that will appear in the set in order left to right.")]
        [SerializeField]
        protected TargetType[] setOrder;

        [SerializeField]
        protected Transform targetTrack;

        [SerializeField]
        private int targetPoints = 25;

        [SerializeField]
        private int decoyPoints = -25;

        [SerializeField]
        protected TargetRack targetRack;

        [SerializeField]
        protected float distanceBetweenTargets = 5.0f;
        
        protected TargetSetState currentState = TargetSetState.Inactive;
        protected Vector3 direction;

        protected ShootingTarget[] shootingTargets;

        protected int totalTargets = 0;
        protected int totalDecoys = 0;
        protected int targetsHit = 0;

        public UnityAction onTargetHit;
        public UnityAction<bool> onTargetSetComplete;

        public int TotalTargets => totalTargets;
        public int TotalDecoys => totalDecoys;

        public bool IsDecoyOnly()
        {
            return totalTargets <= 0 && totalDecoys > 0;
        }

        protected virtual void Awake()
        {
            DetermineTypeCounts();
        }

        protected virtual void Start()
        {
            direction = targetRack.GetRackDirection();
            shootingTargets = new ShootingTarget[setOrder.Length];
            targetTrack.position = targetRack.GetStartPoint();
        }

        private void Update()
        {
            switch (currentState)
            {
                case TargetSetState.Inactive:
                    break;
                case TargetSetState.Ready:
                    break;
                case TargetSetState.Active:
                    ExecuteMainSequence();
                    break;
                case TargetSetState.Terminating:
                    ExecuteStopSequence();
                    break;
                case TargetSetState.Stopped:
                    break;
            }
        }

        private void CheckAllTargetsHit()
        {
            if (targetsHit >= totalTargets)
            {
                StopTargetSet();
            }
        }

        /// <summary>
        /// Get targets from target pool and spawn them.
        /// </summary>
        public void InitiateTargetSet()
        {
            if (currentState != TargetSetState.Ready) return;

            if (shootingTargets.Length == 0)
            {
                ReleaseTargetSet();
                return;
            }

            ResetTargetSet();
            SpawnTargets();
            currentState = TargetSetState.Active;
        }

        /// <summary>
        /// Determines how many normal targets and decoys are in this target set.
        /// </summary>
        private void DetermineTypeCounts()
        {
            foreach (TargetType type in setOrder)
            {
                if (type == TargetType.Normal)
                {
                    totalTargets++;
                }
                else if (type == TargetType.Decoy)
                {
                    totalDecoys++;
                }
            }
        }

        /// <summary>
        /// Allocate targets from target pool to a local list. This should be
        /// done only once per GalleryRound during the round timer.
        /// </summary>
        public void AssignTargets()
        {
            if (currentState != TargetSetState.Inactive) return;

            for (int i = 0; i < setOrder.Length; i++)
            {
                TargetType type = setOrder[i]; 
                if (type == TargetType.Normal)
                {
                    shootingTargets[i] = targetPool.AllocateTarget(this);
                }
                else if (type == TargetType.Decoy)
                {
                    shootingTargets[i] = targetPool.AllocateDecoy(this);
                }

                shootingTargets[i].transform.parent = targetTrack;
            }

            currentState = TargetSetState.Ready;
        }

        public void UnassignTargets()
        {
            RemoveTargets();
        }

        public int GetTotalTargetSetScore()
        {
            return totalTargets * targetPoints;
        }

        /// <summary>
        /// Spawn targets at incremental distances out of the player's view.
        /// </summary>
        private void SpawnTargets()
        {
            for (int i = 0; i < shootingTargets.Length; i++)
            {
                Vector3 spawnPoint = targetRack.GetStartPoint() - direction * (i * distanceBetweenTargets);
                shootingTargets[i].transform.position = spawnPoint;
                shootingTargets[i].gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Clears shooting targets and informs round set that stop sequence has been completed.
        /// </summary>
        private void ReleaseTargetSet()
        {
            for (int i = 0; i < shootingTargets.Length; i++)
            {
                shootingTargets[i] = null;
            }

            currentState = TargetSetState.Inactive;
            onTargetSetComplete?.Invoke(IsDecoyOnly());
        }

        private void UpdateAccuracy()
        {
            if (AccuracyLocator.GetAccuracyTracker() != null)
            {
                AccuracyLocator.GetAccuracyTracker().IncrementTargetsHit();
            }
        }

        private void UpdateScore(int points)
        {
            if (ScoreLocator.GetScoreTracker() != null)
            {
                ScoreLocator.GetScoreTracker().AddToScore(points);
            }
        }

        /// <summary>
        /// Initiates the stop sequence.
        /// </summary>
        public void StopTargetSet()
        {
            if (currentState == TargetSetState.Ready)
            {
                RemoveTargets();
            }
            else if (currentState == TargetSetState.Active)
            {
                currentState = TargetSetState.Terminating;
            }
        }

        /// <summary>
        /// Returns targets to Target Pool. Should be called after targets are
        /// out of the player's view
        /// </summary>
        protected void RemoveTargets()
        {
            foreach (ShootingTarget target in shootingTargets)
            {
                targetPool.DeallocateShootingTarget(target);
            }

            ReleaseTargetSet();
        }

        /// <summary>
        /// Execute the default behavior of the target set.
        /// </summary>
        protected virtual void ExecuteMainSequence()
        {

        }

        /// <summary>
        /// Execute the move-out-of-view behavior of the target set to allow it to
        /// be released.
        /// </summary>
        protected virtual void ExecuteStopSequence()
        {

        }

        /// <summary>
        /// Reset target set and any of its parameters.
        /// </summary>
        protected virtual void ResetTargetSet()
        {
            targetsHit = 0;
        }

        /// <summary>
        /// Moves target track toward the target position at the given speed and direction.
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <param name="currentDirection"></param>
        /// <param name="speed"></param>
        protected void TranslateTrack(Vector3 targetPosition, Vector3 currentDirection, float speed)
        {
            speed = speed * Time.deltaTime;
            if (speed >= (targetPosition - targetTrack.position).magnitude)
            {
                speed = (targetPosition - targetTrack.position).magnitude;
            }

            targetTrack.transform.Translate(currentDirection * speed);
        }

        /// <summary>
        /// Inform round set on points awarded for successfully hitting a target.
        /// </summary>
        /// <param name="points">Points from target</param>
        /// <param name="type">Type of target</param>
        public void OnTargetHit(TargetType type)
        {
            int points;
            if (type == TargetType.Normal)
            {
                targetsHit++;
                points = targetPoints;
                UpdateAccuracy();
                CheckAllTargetsHit();
            }
            else
            {
                points = decoyPoints;
            }

            UpdateScore(points);
            onTargetHit?.Invoke();
        }
    }
}