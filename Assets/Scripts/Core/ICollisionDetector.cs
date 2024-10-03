using System.Collections.Generic;
using Core.Element;

namespace Core
{
    public interface ICollisionDetector
    {
        List<GridGameElement> GetAllRegisteredElements();
        GridGameElement GetGridGameElementByID(int id);
        void Add(GridGameElement element);
        bool Contains(int id);
        void Remove(int id);
        void Clear();
    }
}