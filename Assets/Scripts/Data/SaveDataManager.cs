using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShootingGallery.Data
{
    public class SaveDataManager : MonoBehaviour
    {
        [SerializeField]
        private string saveFileName;

        private GameData gameData;

        private List<ISaveable> saveableObjects;

        private DataFileHandler dataHandler;

        public static SaveDataManager instance { get; private set; }

        private void Awake()
        {
            instance = this;
            dataHandler = new DataFileHandler(Application.persistentDataPath, saveFileName);
            saveableObjects = FindAllSaveableObjects();
            LoadGame();
        }

        private List<ISaveable> FindAllSaveableObjects()
        {
            List<ISaveable> saveables = new List<ISaveable>();

            // TODO: Since only two objects are really saveable in this project. Consider alternatives
            // that might be more efficient such as including a Serialize Field List of Saveable game objects
            foreach (MonoBehaviour sceneObject in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
            {
                if (sceneObject is ISaveable)
                {
                    saveables.Add(sceneObject as ISaveable);
                }
            }

            return saveables;
        }

        public void LoadGame()
        {
            gameData = dataHandler.Load();

            if (gameData == null)
            {
                gameData = new GameData();
            }

            foreach (ISaveable saveable in saveableObjects)
            {
                saveable.LoadData(gameData);
            }
        }

        public void SaveGame()
        {
            foreach (ISaveable saveable in saveableObjects)
            {
                saveable.SaveData(ref gameData);
            }

            dataHandler.Save(gameData);
        }

        private void OnApplicationQuit()
        {
            SaveGame();
        }
    }
}
