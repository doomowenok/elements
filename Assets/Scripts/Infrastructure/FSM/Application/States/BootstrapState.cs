using Core.UI.Level;
using Cysharp.Threading.Tasks;
using Infrastructure.UI;
using Infrastructure.UI.Factory;

namespace Infrastructure.FSM.Application.States
{
    public sealed class BootstrapState : IState
    {
        private readonly IApplicationStateMachine _stateMachine;

        public BootstrapState(IApplicationStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        
        public async UniTask Enter()
        {
            UnityEngine.Application.targetFrameRate = 60;
            
            await _stateMachine.Enter<LoadLevelState>();
        }

        public UniTask Exit()
        {
            return UniTask.CompletedTask;
        }
    }
}