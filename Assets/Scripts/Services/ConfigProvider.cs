using UnityEngine;

namespace Services
{
    public sealed class ConfigProvider : IConfigProvider
    {
        private const string ConfigPath = "Configs";

        public TConfig GetConfig<TConfig>() where TConfig : ScriptableObject =>
            Resources.LoadAll<TConfig>(ConfigPath)[0];
    }
}