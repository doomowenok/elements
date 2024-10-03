using System.Collections.Generic;
using UnityEngine;

namespace Services.Pool
{
    public abstract class ObjectPool<TObject> where TObject : MonoBehaviour, IPoolObject<TObject>
    {
        private static readonly Vector3 PoolObjectPosition = new Vector3(1000, 1000, 1000);
        
        private readonly Stack<IPoolObject<TObject>> _objects = new Stack<IPoolObject<TObject>>();

        public virtual bool TryGet(out IPoolObject<TObject> obj) => 
            _objects.TryPop(out obj);

        public virtual void Return(IPoolObject<TObject> obj)
        {
            obj.ReturnToPool();
            obj.PoolObject.transform.position = PoolObjectPosition;
            _objects.Push(obj);
        }
    }
}