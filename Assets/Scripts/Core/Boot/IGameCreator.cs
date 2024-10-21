using Core.Session;
using JetBrains.Annotations;

namespace Core.Boot
{
    public interface IGameCreator
    {
        void CreateGame(int level, [CanBeNull] SessionSaveData saveData);
    }
}