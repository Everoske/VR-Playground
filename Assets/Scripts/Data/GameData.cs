using UnityEngine;

namespace ShootingGallery.Data
{
    [System.Serializable]
    public class GameData
    {
        // TODO: This is temporary. There should be a high score for each game set.
        public int highScore;

        public GameData()
        {
            this.highScore = 0;
        }
    }
}