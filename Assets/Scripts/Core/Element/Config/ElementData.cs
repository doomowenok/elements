using System;
using UnityEngine;

namespace Core.Element.Config
{
    [Serializable]
    public sealed class ElementData
    {
        public ElementType Type;
        public GridGameElement Prefab;
        public AnimatorOverrideController AnimatorOverrideController;
        public Sprite Sprite;
    }
}