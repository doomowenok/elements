using Infrastructure.FSM.Application.States;
using VContainer;

namespace Infrastructure.FSM.Factory
{
    public sealed class StateFactory : IStateFactory
    {
        private readonly IObjectResolver _resolver;

        public StateFactory(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        public TState CreateState<TState>() where TState : IState => 
            _resolver.Resolve<TState>();
    }
}