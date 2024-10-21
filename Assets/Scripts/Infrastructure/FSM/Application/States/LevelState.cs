using Core.Grid;
using Core.Input;
using Core.UI.Level;
using Cysharp.Threading.Tasks;
using Infrastructure.UI;
using Infrastructure.UI.Factory;
using Infrastructure.UI.MVVM;

namespace Infrastructure.FSM.Application.States
{
    public sealed class LevelState : IState
    {
        private readonly IInputSystem _inputSystem;
        private readonly IGridElementController _gridElementController;
        private readonly IUIController _uiController;

        public LevelState(
            IInputSystem inputSystem,
            IGridElementController gridElementController, 
            IUIController uiController)
        {
            _inputSystem = inputSystem;
            _gridElementController = gridElementController;
            _uiController = uiController;
        }
        
        public UniTask Enter()
        {
            _inputSystem.EnableInput();
            _gridElementController.Initialize();

            _uiController.CreateView<LevelView>(UIViewTypes.Level);
            _uiController.SubscribeViewModel<LevelViewModel>();

            return UniTask.CompletedTask;
        }

        public UniTask Exit()
        {
            _inputSystem.DisableInput();
            _uiController.UnsubscribeViewModel<LevelViewModel>();
            return UniTask.CompletedTask;
        }
    }
}