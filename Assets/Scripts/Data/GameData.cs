using System.Collections.Generic;
using UnityEngine;

namespace ShootingGallery.Data
{
    [System.Serializable]
    public class GameData
    {
        public SerializableDictionary<string, int> highScoreData;
        public float masterVolume;
        public float musicVolume;
        public float sfxVolume;
        public bool showAmmoCounter;

        public GameData()
        {
            highScoreData = new SerializableDictionary<string, int>();
        }
    }
}