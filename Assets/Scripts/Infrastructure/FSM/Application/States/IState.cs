using Cysharp.Threading.Tasks;

namespace Infrastructure.FSM.Application.States
{
    public interface IState
    {
        UniTask Enter();
        UniTask Exit();
    }
}