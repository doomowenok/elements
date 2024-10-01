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
        
        [SerializeField] private List<ElementPrefabData> _elementPrefabsData;
        private Dictionary<ElementType, GridGameElement> _elementPrefabs;

        public IReadOnlyDictionary<ElementType, GridGameElement> ElementPrefabs
        {
            get
            {
                if (_elementPrefabs == null || _elementPrefabs.Count == 0)
                {
                    _elementPrefabs = _elementPrefabsData
                        .ToDictionary(key => key.Type, value => value.Prefab);
                }

                return _elementPrefabs;
            }
        }
    }
}