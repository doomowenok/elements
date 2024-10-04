using Core.Grid;
using Core.Input;
using Core.UI.Level;
using Cysharp.Threading.Tasks;
using Infrastructure.UI;

namespace Infrastructure.FSM.Application.States
{
    public sealed class LevelState : IState
    {
        private readonly IInputSystem _inputSystem;
        private readonly IGridElementController _gridElementController;
        private readonly IUIController _uiController;

        public LevelState(IInputSystem inputSystem, IGridElementController gridElementController, IUIController uiController)
        {
            _inputSystem = inputSystem;
            _gridElementController = gridElementController;
            _uiController = uiController;
        }
        
        public UniTask Enter()
        {
            _inputSystem.EnableInput();
            _gridElementController.Initialize();
            
            if (!_uiController.IsOpen<LevelWindow>())
            {
                _uiController.GetWindow<LevelWindow>().Show();
            }

            _uiController.SetInteractionState<LevelWindow>(true);
            
            return UniTask.CompletedTask;
        }

        public UniTask Exit()
        {
            _uiController.SetInteractionState<LevelWindow>(false);
            _inputSystem.DisableInput();
            return UniTask.CompletedTask;
        }
    }
}