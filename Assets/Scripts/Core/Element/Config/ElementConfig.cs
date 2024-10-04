using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Element.Config
{
    [CreateAssetMenu(fileName = nameof(ElementConfig), menuName = "Configs/Core/Element Config")]
    public sealed class ElementConfig : ScriptableObject
    {
        [SerializeField] private Vector2 _elementSize = Vector2.one * 2.0f;
        public Vector2 ElementSize => _elementSize;
        
        [SerializeField] public GridGameElement _prefab;
        public GridGameElement Prefab => _prefab;
        
        [SerializeField] private List<ElementData> _elementPrefabsData;
        private Dictionary<ElementType, ElementData> _elementPrefabs;
        public IReadOnlyDictionary<ElementType, ElementData> ElementPrefabs
        {
            get
            {
                if (_elementPrefabs == null || _elementPrefabs.Count == 0)
                {
                    _elementPrefabs = _elementPrefabsData
                        .ToDictionary(key => key.Type);
                }

                return _elementPrefabs;
            }
        }

        [SerializeField] private float _moveTimeAcrossGridSpeed = 0.2f;
        public float MoveTimeAcrossGridSpeed => _moveTimeAcrossGridSpeed;
    }
}