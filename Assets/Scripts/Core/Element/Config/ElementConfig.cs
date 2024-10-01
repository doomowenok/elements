using UnityEngine;

namespace Core.Element.Config
{
    [CreateAssetMenu(fileName = nameof(ElementConfig), menuName = "Configs/Core/Element Config")]
    public sealed class ElementConfig : ScriptableObject
    {
        [SerializeField] private Vector2 _elementSize = Vector2.one * 2.0f;
        public Vector2 ElementSize => _elementSize;
        
    }
}