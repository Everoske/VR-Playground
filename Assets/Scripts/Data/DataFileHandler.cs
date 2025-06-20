using UnityEngine;
using System;
using System.IO;

namespace ShootingGallery.Data
{
    public class DataFileHandler
    {
        private string dataDirectoryPath = "";
        private string dataFileName = "";

        public DataFileHandler(string dataDirectoryPath, string dataFileName)
        {
            this.dataDirectoryPath = dataDirectoryPath;
            this.dataFileName = dataFileName;
        }

        public GameData Load()
        {
            string fullPath = Path.Combine(dataDirectoryPath, dataFileName);
            GameData loadedData = null;

            if (File.Exists(fullPath))
            {
                try
                {
                    string dataToLoad = "";
                    using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            dataToLoad = reader.ReadToEnd();
                        }
                    }

                    loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }

            return loadedData;
        }

        public void Save(GameData data)
        {
            string fullPath = Path.Combine(dataDirectoryPath, dataFileName);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath)); 

                string dataToStore = JsonUtility.ToJson(data, true);

                using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(dataToStore);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
