using UnityEngine;

namespace ShootingGallery.Game
{
    /// <summary>
    /// Handles spawning guns in at the start of a game set and respawning guns
    /// that go out of bounds
    /// </summary>
    public class GunDrawer : MonoBehaviour
    {
        [SerializeField]
        private Transform[] largeGunSpawns;

        [SerializeField]
        private Transform[] smallGunSpawns;
        
        private Animator animator;

        private const string animatorOpenRef = "Open";

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }


        public void SpawnGuns()
        {
            // Receive guns to spawn from GameSetManager

            // Spawn them at a random unused location based on available spawns

            // Add guns to internal array/list for tracking

            // Once guns have spawned, open drawer
            OpenDrawer();
        }

        public void DespawnGuns()
        {
            // Assign a new tag to all guns so they cannot be used by the player

            // Move guns back into gun drawer

            // Close gun drawer
            CloseDrawer();
        }

        public void OnDrawerClosed()
        {
            // Once drawer closes, despawn each gun
            // Allow new guns to be spawned
        }

        private void OpenDrawer()
        {
            animator.SetBool(animatorOpenRef, true);
        }

        private void CloseDrawer()
        {
            animator.SetBool(animatorOpenRef, false);
        }
    }
}
