using ShootingGallery.Data;
using UnityEngine;

namespace ShootingGallery.Data
{
    public interface ISaveable
    {
        void LoadData(GameData data);
        void SaveData(ref GameData data);
    }
}