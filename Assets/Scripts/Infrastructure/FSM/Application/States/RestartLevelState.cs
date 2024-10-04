using Core.Boot;
using Core.Session;
using Core.Utils;
using Cysharp.Threading.Tasks;

namespace Infrastructure.FSM.Application.States
{
    public sealed class RestartLevelState : IState
    {
        private readonly IGameplayDisposer _gameplayDisposer;
        private readonly SessionController _sessionController;
        private readonly IGameCreator _gameCreator;
        private readonly IApplicationStateMachine _stateMachine;

        public RestartLevelState(
            IGameplayDisposer gameplayDisposer,
            SessionController sessionController,
            IGameCreator gameCreator,
            IApplicationStateMachine stateMachine)
        {
            _gameplayDisposer = gameplayDisposer;
            _sessionController = sessionController;
            _gameCreator = gameCreator;
            _stateMachine = stateMachine;
        }
        
        public async UniTask Enter()
        {
            await _gameplayDisposer.DisposeGameAsync();
            _gameCreator.CreateGame(_sessionController.Level);
            await _stateMachine.Enter<LevelState>();
        }

        public UniTask Exit()
        {
            return UniTask.CompletedTask;
        }
    }
}