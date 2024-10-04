using UnityEngine;

namespace Extensions
{
    public static class VectorExtensions
    {
        public static float GetRandomInRange(this Vector2 range) => Random.Range(range.x, range.y);
    }
}