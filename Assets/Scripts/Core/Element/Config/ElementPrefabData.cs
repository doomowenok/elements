using System;

namespace Core.Element.Config
{
    [Serializable]
    public sealed class ElementPrefabData
    {
        public ElementType Type;
        public GridGameElement Prefab;
    }
}