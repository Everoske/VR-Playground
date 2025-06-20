using System.Collections.Generic;
using UnityEngine;

namespace ShootingGallery.Data
{
    [System.Serializable]
    public class GameData
    {
        public SerializableDictionary<string, int> highScoreData;

        public GameData()
        {
            highScoreData = new SerializableDictionary<string, int>();
        }
    }
}