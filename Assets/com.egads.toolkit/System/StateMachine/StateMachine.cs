using System.Collections.Generic;

namespace egads.system.stateMachine
{
	public class StateMachine<T> where T : IState
	{
		#region Private Properties

		private Stack<T> _stack = new Stack<T>();

		#endregion

		#region Public Properties

		public int count => _stack.Count;

		public bool hasMultipleStates => _stack.Count > 1;

		public bool hasState => _stack.Count > 0;

		#endregion

		#region Core Methods

		public void Push(T state)
		{
			if (_stack.Count > 0) { _stack.Peek().OnLostFocus(); }

			_stack.Push(state);
			state.OnEnter();
			state.OnGotFocus();
		}

		public T Pop()
		{
			if (_stack.Count > 0)
			{
				T state = _stack.Pop();
				state.OnLostFocus();
				state.OnExit();

				if (_stack.Count > 0) { Peek().OnGotFocus(); }

				return state;
			}
			else { return default(T); }
		}
        public T Peek()
        {
            if (_stack.Count > 0) { return _stack.Peek(); }
            else { return default(T); }  
        }

        public void SwitchTo(T state)
        {
            PopAll();
            Push(state);
        }

        public void PopAll()
        {
            while (_stack.Count > 0) { Pop(); }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// should be called by the class holding the StateMachine
        /// </summary>
        public void Update()
		{
			if (_stack.Count > 0) { _stack.Peek().OnUpdate(); }
		}

        #endregion
    }
}