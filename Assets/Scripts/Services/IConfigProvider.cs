using UnityEngine;

namespace Services
{
    public interface IConfigProvider
    {
        TConfig GetConfig<TConfig>() where TConfig : ScriptableObject;
    }
}