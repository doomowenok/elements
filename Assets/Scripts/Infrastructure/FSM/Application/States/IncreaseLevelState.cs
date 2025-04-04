using Core.Boot;
using Core.Session;
using Core.Utils;
using Cysharp.Threading.Tasks;

namespace Infrastructure.FSM.Application.States
{
    public sealed class IncreaseLevelState : IState
    {
        private readonly IGameCreator _gameCreator;
        private readonly SessionController _sessionController;
        private readonly IGameplayDisposer _gameplayDisposer;
        private readonly IApplicationStateMachine _stateMachine;

        public IncreaseLevelState(
            IGameCreator gameCreator,
            SessionController sessionController,
            IGameplayDisposer gameplayDisposer,
            IApplicationStateMachine stateMachine)
        {
            _gameCreator = gameCreator;
            _sessionController = sessionController;
            _gameplayDisposer = gameplayDisposer;
            _stateMachine = stateMachine;
        }
        
        public async UniTask Enter()
        {
            await _gameplayDisposer.DisposeGameAsync();
            _sessionController.Level.Value++;
            _gameCreator.CreateGame(_sessionController.Level.Value, null);
            await _stateMachine.Enter<LevelState>();
        }

        public UniTask Exit()
        {
            return UniTask.CompletedTask;
        }
    }
}