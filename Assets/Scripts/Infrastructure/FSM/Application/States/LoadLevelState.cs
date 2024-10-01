using Core;
using Cysharp.Threading.Tasks;
using Services.Scene;

namespace Infrastructure.FSM.Application.States
{
    public sealed class LoadLevelState : IState
    {
        private readonly IApplicationStateMachine _stateMachine;
        private readonly ISceneLoader _sceneLoader;
        private readonly IGameCreator _gameCreator;

        public LoadLevelState(IApplicationStateMachine stateMachine, ISceneLoader sceneLoader, IGameCreator gameCreator)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
            _gameCreator = gameCreator;
        }
        
        public async UniTask Enter()
        {
            _sceneLoader.LoadSceneAsync("Game", CreateGame);
            await _stateMachine.Enter<LevelState>();
        }

        public UniTask Exit()
        {
            return UniTask.CompletedTask;
        }

        private void CreateGame()
        {
            _gameCreator.CreateGame(0);
        }
    }
}