using System;
using UnityEngine;

namespace Core.Element.Config
{
    [Serializable]
    public sealed class ElementData
    {
        public ElementType Type;
        public AnimatorOverrideController AnimatorOverrideController;
        public Sprite Sprite;
    }
}