using Cysharp.Threading.Tasks;

namespace Core.Utils
{
    public interface IGameplayDisposer
    {
        UniTask DisposeGameAsync();
    }
}