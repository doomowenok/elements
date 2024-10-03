using Cysharp.Threading.Tasks;

namespace Core
{
    public interface IGameplayDisposer
    {
        UniTask DisposeGameAsync();
    }
}