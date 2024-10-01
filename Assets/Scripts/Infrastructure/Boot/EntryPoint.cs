using Cysharp.Threading.Tasks;
using Infrastructure.FSM.Application;
using Infrastructure.FSM.Application.States;
using Infrastructure.FSM.Factory;
using VContainer.Unity;

namespace Infrastructure.Boot
{
    public sealed class EntryPoint : IInitializable
    {
        private readonly IStateFactory _stateFactory;
        private readonly IApplicationStateMachine _stateMachine;

        public EntryPoint(IStateFactory stateFactory, IApplicationStateMachine stateMachine)
        {
            _stateFactory = stateFactory;
            _stateMachine = stateMachine;
        }
        
        public void Initialize()
        {
            _stateMachine.AddState(_stateFactory.CreateState<BootstrapState>());
            _stateMachine.AddState(_stateFactory.CreateState<LoadLevelState>());
            _stateMachine.AddState(_stateFactory.CreateState<LevelState>());

            _stateMachine.Enter<BootstrapState>().Forget();
        }
    }
}