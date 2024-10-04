using UnityEngine;

namespace Core.Utils
{
    public sealed class StartPointsProvider : MonoBehaviour
    {
        [SerializeField] private Transform _startDownPoint;
        public Transform StartDownPoint => _startDownPoint;
    }
}