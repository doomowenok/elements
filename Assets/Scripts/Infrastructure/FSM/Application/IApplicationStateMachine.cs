using Cysharp.Threading.Tasks;
using Infrastructure.FSM.Application.States;

namespace Infrastructure.FSM.Application
{
    public interface IApplicationStateMachine
    {
        void AddState<TState>(TState state) where TState : IState;
        UniTask Enter<TState>() where TState : IState;
    }
}