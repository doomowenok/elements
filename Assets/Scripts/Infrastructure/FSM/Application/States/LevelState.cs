using Core;
using Core.Input;
using Cysharp.Threading.Tasks;

namespace Infrastructure.FSM.Application.States
{
    public sealed class LevelState : IState
    {
        private readonly IInputSystem _inputSystem;
        private readonly IGridElementController _gridElementController;

        public LevelState(IInputSystem inputSystem, IGridElementController gridElementController)
        {
            _inputSystem = inputSystem;
            _gridElementController = gridElementController;
        }
        
        public UniTask Enter()
        {
            _inputSystem.EnableInput();
            _gridElementController.Initialize();
            return UniTask.CompletedTask;
        }

        public UniTask Exit()
        {
            return UniTask.CompletedTask;
        }
    }
}