using System.Collections.Generic;
using Core.Grid.Config;
using UnityEngine;

namespace Core.Config
{
    [CreateAssetMenu(fileName = nameof(GameConfig), menuName = "Configs/Core/Game Config")]
    public sealed class GameConfig : ScriptableObject
    {
        [SerializeField] private List<LevelGridConfig> _levelGridConfigs;
        public IReadOnlyList<LevelGridConfig> LevelGridConfigs => _levelGridConfigs;

        [SerializeField] private float _defaultGridWidth = 10.0f;
        public float DefaultGridWidth => _defaultGridWidth;
    }
}