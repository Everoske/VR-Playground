using System;
using System.Collections.Generic;
using ShootingGallery.Game;
using ShootingGallery.Interfaces;
using ShootingGallery.Settings;
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
        }

        private void Start()
        {
            dataHandler = new DataFileHandler(Application.persistentDataPath, saveFileName);
            saveableObjects = FindAllSaveableObjects();

            LoadGame();
        }

        private List<ISaveable> FindAllSaveableObjects()
        {
            List<ISaveable> saveables = new List<ISaveable>();

            foreach (GameSet gameSet in FindObjectsByType<GameSet>(FindObjectsSortMode.None))
            {
                if (gameSet is ISaveable)
                {
                    saveables.Add(gameSet as ISaveable);
                }
            }

            SettingsManager settingsManager = FindFirstObjectByType<SettingsManager>();
            if (settingsManager is ISaveable)
            {
                saveables.Add(settingsManager as ISaveable);
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
