using UnityEngine;

namespace ShootingGallery.Game
{
    // Represents a series of target sets to spawn
    public class RoundSet : MonoBehaviour
    {

        [SerializeField]
        private float setTime = 10.0f; // Total time to run set

        [SerializeField]
        private float timeBeforeSet = 1.0f; // Time before set
    }
}
