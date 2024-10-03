using System.Collections.Generic;
using System.Linq;
using Core.Element;

namespace Core
{
    public sealed class CollisionDetector : ICollisionDetector
    {
        private readonly Dictionary<int, GridGameElement> _gridGameElements = new Dictionary<int, GridGameElement>();

        public List<GridGameElement> GetAllRegisteredElements() => _gridGameElements.Values.ToList();
        public GridGameElement GetGridGameElementByID(int id) => _gridGameElements[id];
        public void Add(GridGameElement element) => _gridGameElements.Add(element.gameObject.GetInstanceID(), element);
        public bool Contains(int id) => _gridGameElements.ContainsKey(id);

        public void Remove(int id) => _gridGameElements.Remove(id);

        public void Clear() => _gridGameElements.Clear();
    }
}