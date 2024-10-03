using Core.Session;
using Cysharp.Threading.Tasks;

namespace Core
{
    public interface IGameCreator
    {
        void CreateGame(int level);
        void RecoverGame(SessionSaveData saveData);
    }
}