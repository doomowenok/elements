using Extensions.Property;
using Infrastructure.UI.MVVM;
using UnityEngine.Events;

namespace Core.UI.Level
{
    public class LevelViewModel : BaseViewModel<LevelModel>
    {
        public UnityEvent OnNextLevelClick { get; private set; } = new UnityEvent();
        public UnityEvent OnRestartLevelClick { get; private set; } = new UnityEvent();
        public INotifyProperty<int> Level { get; private set; } = new NotifyProperty<int>();

        public LevelViewModel(LevelModel model) : base(model) { }
        
        public override void Subscribe()
        {
            OnNextLevelClick.AddListener(Model.NextLevel);
            OnRestartLevelClick.AddListener(Model.RestartLevel);
            Model.Level.OnChanged += InvokeLevelChanged;
            InvokeLevelChanged(Model.Level.Value);
        }

        public override void Unsubscribe()
        {
            OnNextLevelClick.RemoveListener(Model.NextLevel);
            OnRestartLevelClick.RemoveListener(Model.RestartLevel);
            Model.Level.OnChanged -= InvokeLevelChanged;
        }

        private void InvokeLevelChanged(int level)
        {
            Level.Value = level;
        }
    }
}