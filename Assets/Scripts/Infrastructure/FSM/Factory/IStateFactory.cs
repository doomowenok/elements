using Infrastructure.FSM.Application.States;

namespace Infrastructure.FSM.Factory
{
    public interface IStateFactory
    {
        TState CreateState<TState>() where TState : IState;
    }
}