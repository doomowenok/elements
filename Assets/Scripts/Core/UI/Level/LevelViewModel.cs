using Infrastructure.UI.MVVM;
using UnityEngine.Events;

namespace Core.UI.Level
{
    public class LevelViewModel : BaseViewModel<LevelModel>
    {
        public UnityEvent OnNextLevelClick { get; private set; } = new UnityEvent();
        public UnityEvent OnRestartLevelClick { get; private set; } = new UnityEvent();
        
        public LevelViewModel(LevelModel model) : base(model) { }
        
        public override void Subscribe()
        {
            OnNextLevelClick.AddListener(Model.NextLevel);
            OnRestartLevelClick.AddListener(Model.RestartLevel);
        }

        public override void Unsubscribe()
        {
            OnNextLevelClick.RemoveListener(Model.NextLevel);
            OnRestartLevelClick.RemoveListener(Model.RestartLevel);
        }
    }
}