using Cysharp.Threading.Tasks;

namespace Infrastructure.FSM.Application.States
{
    public sealed class LevelState : IState
    {
        public UniTask Enter()
        {
            return UniTask.CompletedTask;
        }

        public UniTask Exit()
        {
            return UniTask.CompletedTask;
        }
    }
}