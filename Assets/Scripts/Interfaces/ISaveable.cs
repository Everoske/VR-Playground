using ShootingGallery.Data;
using UnityEngine;

namespace ShootingGallery.Interfaces
{
    public interface ISaveable
    {
        void LoadData(GameData data);
        void SaveData(ref GameData data);
    }
}