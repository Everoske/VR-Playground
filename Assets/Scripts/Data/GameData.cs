using ShootingGallery.XR;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

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
        public TurnType turnType;
        public InteractorHandedness turnHandedness;
        public InteractorHandedness moveHandedness;

        public GameData()
        {
            highScoreData = new SerializableDictionary<string, int>();
        }
    }
}