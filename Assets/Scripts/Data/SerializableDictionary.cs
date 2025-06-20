using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ShootingGallery.Data
{
    [System.Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> keys = new List<TKey>();
        [SerializeField]
        private List<TValue> values = new List<TValue>();

        public void OnAfterDeserialize()
        {
            Clear();

            if (keys.Count != values.Count)
            {
                Debug.LogError("Serializable Dictionary: Unable to deserialize. The number of keys and values do not match!");
            }

            for (int i = 0; i < keys.Count; i++)
            {
                Add(keys[i], values[i]);
            }
        }

        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }
    }
}
