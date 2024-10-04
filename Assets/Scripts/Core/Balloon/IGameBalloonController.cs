using Cysharp.Threading.Tasks;

namespace Core.Balloon
{
    public interface IGameBalloonController
    {
        UniTask StartCreatingBalloons();
    }
}