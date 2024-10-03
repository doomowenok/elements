using Core;
using Core.Session;
using Cysharp.Threading.Tasks;
using Services.Save;
using Services.Scene;

namespace Infrastructure.FSM.Application.States
{
    public sealed class LoadLevelState : IState
    {
        private readonly IApplicationStateMachine _stateMachine;
        private readonly ISceneLoader _sceneLoader;
        private readonly IGameCreator _gameCreator;
        private readonly ISaveService _saveService;

        public LoadLevelState(
            IApplicationStateMachine stateMachine, 
            ISceneLoader sceneLoader,
            IGameCreator gameCreator, 
            ISaveService saveService)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
            _gameCreator = gameCreator;
            _saveService = saveService;
        }
        
        public async UniTask Enter()
        {
            await _sceneLoader.LoadSceneAsync("Game", () => CreateGame().Forget());
        }

        public UniTask Exit()
        {
            return UniTask.CompletedTask;
        }

        private async UniTask CreateGame()
        {
            if (_saveService.ContainsSave<SessionSaveData>())
            {
                SessionSaveData saveData = _saveService.Load<SessionSaveData>();
                _gameCreator.RecoverGame(saveData);
            }
            else
            {
                _gameCreator.CreateGame(0);
            }
            
            await _stateMachine.Enter<LevelState>();
        }
    }
}