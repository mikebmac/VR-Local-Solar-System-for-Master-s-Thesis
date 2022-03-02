using UnityEngine;

namespace MacKay.PlayerController.Ship
{
    public abstract class ShipBaseState
    {
        private bool _isRootState = false;
        protected bool IsRootState
        {
            set => _isRootState = value;
        }
        private ShipStateMachine _ctx;
        protected ShipStateMachine Ctx => _ctx;
        private ShipStateFactory _factory;
        protected ShipStateFactory Factory => _factory;
        private ShipBaseState _currentSuperState;
        public ShipBaseState CurrentSuperState => _currentSuperState;
        private ShipBaseState _currentSubState;
        public ShipBaseState CurrentSubState => _currentSubState;

        protected ShipBaseState(ShipStateMachine currentContext, ShipStateFactory shipStateFactory)
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

        protected void SwitchState(ShipBaseState newState)
        {

            if (newState._isRootState)
            {
                _ctx.CurrentState.ExitState();
                _ctx.CurrentState = newState;
            }
            else if (_currentSuperState != null)
            {
                _ctx.CurrentState._currentSubState.ExitState();
                _currentSuperState.SetSubState(newState);
            }
            
            newState.EnterState();

        }

        protected void SetSuperState(ShipBaseState newSuperState)
        {
            _currentSuperState = newSuperState;
            
        }

        protected void SetSubState(ShipBaseState newSubState)
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