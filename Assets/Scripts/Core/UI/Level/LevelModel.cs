using Cysharp.Threading.Tasks;
using Infrastructure.FSM.Application;
using Infrastructure.FSM.Application.States;
using Infrastructure.UI.MVVM;

namespace Core.UI.Level
{
    public sealed class LevelModel : BaseModel
    {
        private readonly IApplicationStateMachine _applicationStateMachine;

        public LevelModel(IApplicationStateMachine applicationStateMachine)
        {
            _applicationStateMachine = applicationStateMachine;
        }

        public void NextLevel()
        {
            _applicationStateMachine.Enter<IncreaseLevelState>().Forget();
        }

        public void RestartLevel()
        {
            _applicationStateMachine.Enter<RestartLevelState>().Forget();
        }
    }
}