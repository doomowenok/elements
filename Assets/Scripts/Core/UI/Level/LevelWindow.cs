using Cysharp.Threading.Tasks;
using Infrastructure.FSM.Application;
using Infrastructure.FSM.Application.States;
using Infrastructure.UI;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Core.UI.Level
{
    public sealed class LevelWindow : BaseWindow
    {
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private Button _restartLevel;
        
        private IApplicationStateMachine _stateMachine;

        [Inject]
        private void Construct(IApplicationStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        
        public override void Show()
        {
            _nextLevelButton.onClick.AddListener(MoveToNextLevel);
            _restartLevel.onClick.AddListener(RestartLevel);
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
            _nextLevelButton.onClick.RemoveAllListeners();
            _restartLevel.onClick.RemoveAllListeners();
        }

        private void MoveToNextLevel()
        {
            if (CanInteract)
            {
                _stateMachine.Enter<IncreaseLevelState>().Forget();
            }
        }

        private void RestartLevel()
        {
            if (CanInteract)
            {
                _stateMachine.Enter<RestartLevelState>().Forget();
            }
        }
    }
}