using Core;
using Core.Input;
using Cysharp.Threading.Tasks;

namespace Infrastructure.FSM.Application.States
{
    public sealed class LevelState : IState
    {
        private readonly IInputSystem _inputSystem;
        private readonly IGridElementMover _gridElementMover;

        public LevelState(IInputSystem inputSystem, IGridElementMover gridElementMover)
        {
            _inputSystem = inputSystem;
            _gridElementMover = gridElementMover;
        }
        
        public UniTask Enter()
        {
            _inputSystem.EnableInput();
            _gridElementMover.Initialize();
            return UniTask.CompletedTask;
        }

        public UniTask Exit()
        {
            return UniTask.CompletedTask;
        }
    }
}