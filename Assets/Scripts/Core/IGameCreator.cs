using Core.Session;
using Cysharp.Threading.Tasks;

namespace Core
{
    public interface IGameCreator
    {
        UniTask CreateGame(int level);
        UniTask RecoverGame(SessionSaveData saveData);
    }
}