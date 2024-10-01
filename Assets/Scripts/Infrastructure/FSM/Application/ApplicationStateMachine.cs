using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Infrastructure.FSM.Application.States;

namespace Infrastructure.FSM.Application
{
    public sealed class ApplicationStateMachine : IApplicationStateMachine
    {
        private readonly IDictionary<Type, IState> _states = new Dictionary<Type, IState>();
        
        private IState _currentState;

        public void AddState<TState>(TState state) where TState : IState
        {
            _states.Add(typeof(TState), state);
        }

        public async UniTask Enter<TState>() where TState : IState
        {
            if (_currentState != null)
            {
                await _currentState.Exit();
            }
            
            _currentState = _states[typeof(TState)];
            
            await _currentState.Enter();
        }
    }
}