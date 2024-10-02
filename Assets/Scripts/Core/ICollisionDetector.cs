using Core.Element;

namespace Core
{
    public interface ICollisionDetector
    {
        GridGameElement GetGridGameElementByID(int id);
        void Add(GridGameElement element);
        bool Contains(int id);
        void Remove(int id);
        void Clear();
    }
}