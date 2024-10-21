using Infrastructure.UI.MVVM;
using TMPro;
using UnityEngine.UI;

namespace Core.UI.Level
{
    public sealed class LevelView : BaseView<LevelViewModel>
    {
        public TMP_Text levelText;
        public Button nextLevelButton;
        public Button restartLevel;
        
        public override void Subscribe()
        {
            nextLevelButton.onClick.AddListener(ViewModel.OnNextLevelClick.Invoke);
            restartLevel.onClick.AddListener(ViewModel.OnRestartLevelClick.Invoke);
            ViewModel.Level.OnChanged += UpdateText;
            UpdateText(ViewModel.Level.Value);
        }

        public override void Unsubscribe()
        {
            nextLevelButton.onClick.RemoveListener(ViewModel.OnNextLevelClick.Invoke);
            restartLevel.onClick.RemoveListener(ViewModel.OnRestartLevelClick.Invoke);
            ViewModel.Level.OnChanged -= UpdateText;
        }

        private void UpdateText(int level)
        {
            levelText.text = $"Level: {level}";
        }
    }
}