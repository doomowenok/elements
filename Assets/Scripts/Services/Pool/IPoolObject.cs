using UnityEngine;

namespace Services.Pool
{
    public interface IPoolObject<out TObject> where TObject : MonoBehaviour
    {
        TObject PoolObject { get; }
        void ReturnToPool();
    }
}