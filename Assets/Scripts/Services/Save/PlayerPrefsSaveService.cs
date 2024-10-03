using UnityEngine;

namespace Services.Save
{
    public sealed class PlayerPrefsSaveService : ISaveService
    {
        public void Save<TData>(TData data) where TData : class
        {
            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(typeof(TData).Name, json);
            PlayerPrefs.Save();
        }

        public TData Load<TData>() where TData : class
        {
            string json = PlayerPrefs.GetString(typeof(TData).Name);
            TData data = JsonUtility.FromJson<TData>(json);
            return data;
        }

        public bool ContainsSave<TData>() where TData : class => 
            PlayerPrefs.HasKey(typeof(TData).Name);
    }
}