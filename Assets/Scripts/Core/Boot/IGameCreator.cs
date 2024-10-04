using Core.Session;

namespace Core.Boot
{
    public interface IGameCreator
    {
        void CreateGame(int level);
        void RecoverGame(SessionSaveData saveData);
    }
}