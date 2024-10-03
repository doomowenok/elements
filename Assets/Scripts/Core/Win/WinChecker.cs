using Core.Session;
using Cysharp.Threading.Tasks;
using Infrastructure.FSM.Application;
using Infrastructure.FSM.Application.States;

namespace Core.Win
{
    public sealed class WinChecker : IWinChecker
    {
        private readonly SessionController _sessionController;
        private readonly IApplicationStateMachine _stateMachine;

        public WinChecker(
            SessionController sessionController,
            IApplicationStateMachine stateMachine)
        {
            _sessionController = sessionController;
            _stateMachine = stateMachine;
        }
        
        public void CheckWin()
        {
            if (_sessionController.IsElementsEmpty())
            {
                _stateMachine.Enter<IncreaseLevelState>().Forget();
            }
        }
    }
}