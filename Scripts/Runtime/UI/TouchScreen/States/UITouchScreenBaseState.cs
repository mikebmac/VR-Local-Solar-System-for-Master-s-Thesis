using UnityEngine;

namespace MacKay.UI
{
    public abstract class UITouchScreenBaseState
    {
        private bool _isRootState = false;

        protected bool IsRootState
        {
            set => _isRootState = value;
        }

        private UITouchStateMachine _ctx;
        protected UITouchStateMachine Ctx => _ctx;
        private UITouchScreenStateFactory _factory;
        protected UITouchScreenStateFactory Factory => _factory;
        private UITouchScreenBaseState _currentSuperState;
        public UITouchScreenBaseState CurrentSuperState => _currentSuperState;
        private UITouchScreenBaseState _currentSubState;
        public UITouchScreenBaseState CurrentSubState => _currentSubState;

        protected UITouchScreenBaseState(UITouchStateMachine currentContext, UITouchScreenStateFactory shipStateFactory)
        {
            _ctx = currentContext;
            _factory = shipStateFactory;
        }

        public abstract void EnterState();
        public abstract void UpdateState();

        public abstract void ExitState();
        public abstract void CheckSwitchStates();
        public abstract void InitializeSubState();

        public void UpdateStates()
        {
            UpdateState();
            CheckSwitchStates();
            if (_currentSubState != null) _currentSubState.UpdateStates();
        }

        protected void SwitchState(UITouchScreenBaseState newState)
        {
            ExitState();

            if (_isRootState)
            {
                _ctx.CurrentState = newState;
            }
            else if (_currentSuperState != null)
            {
                _currentSuperState.SetSubState(newState);
            }
            
            newState.EnterState();
        }

        protected void SetSuperState(UITouchScreenBaseState newSuperState)
        {
            _currentSuperState = newSuperState;
        }

        protected void SetSubState(UITouchScreenBaseState newSubState)
        {
            _currentSubState?.ExitState();

            _currentSubState = newSubState;
            newSubState.SetSuperState(this);
        }

        protected void Log(string msg)
        {
            if (!_ctx._enableDebug) return;

            Debug.Log($"<color=green>[{GetType()}]</color>: {msg}", _ctx);
        }

        protected void LogWarning(string msg, bool overrideSettings = false)
        {
            if (!_ctx._enableDebug && !overrideSettings) return;
            Debug.LogWarning($"<color=red>[{GetType()}]</color>: {msg}", _ctx);
        }
    }
}