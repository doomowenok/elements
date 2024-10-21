using Infrastructure.UI.MVVM;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.Level
{
    public sealed class LevelView : BaseView<LevelViewModel>
    {
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private Button _restartLevel;
        
        public override void Subscribe()
        {
            _nextLevelButton.onClick.AddListener(ViewModel.OnNextLevelClick.Invoke);
            _restartLevel.onClick.AddListener(ViewModel.OnRestartLevelClick.Invoke);
        }

        public override void Unsubscribe()
        {
            _nextLevelButton.onClick.RemoveListener(ViewModel.OnNextLevelClick.Invoke);
            _restartLevel.onClick.RemoveListener(ViewModel.OnRestartLevelClick.Invoke);
        }
    }
}