using System;
using Core.Session;
using Cysharp.Threading.Tasks;
using Extensions.Property;
using Infrastructure.FSM.Application;
using Infrastructure.FSM.Application.States;
using Infrastructure.UI.MVVM;
using UnityEngine;
using VContainer.Unity;

namespace Core.UI.Level
{
    public sealed class LevelModel : BaseModel, IInitializable, IDisposable
    {
        private readonly IApplicationStateMachine _applicationStateMachine;
        private readonly SessionController _sessionController;

        public INotifyProperty<int> Level { get; private set; } = new NotifyProperty<int>();

        public LevelModel(IApplicationStateMachine applicationStateMachine, SessionController sessionController)
        {
            _applicationStateMachine = applicationStateMachine;
            _sessionController = sessionController;
        }

        void IInitializable.Initialize()
        {
            _sessionController.Level.OnChanged += UpdateLevel;
            UpdateLevel(_sessionController.Level.Value);
        }

        void IDisposable.Dispose()
        {
            _sessionController.Level.OnChanged -= UpdateLevel;
        }

        private void UpdateLevel(int level)
        {
            Level.Value = level;
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