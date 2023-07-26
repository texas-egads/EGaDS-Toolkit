using System.Collections.Generic;

namespace egads.system.UI
{
	public class UIStateMachine<T>
	{
        #region Private Properties

        private Stack<IUIState<T>> _stack = new Stack<IUIState<T>>();

        #endregion

        #region Public Properties

        public int count => _stack.Count;
		public bool hasMultipleStates => _stack.Count > 1;
		public bool hasState => _stack.Count > 0;

        #endregion

        #region Public Methods
        public IUIState<T> current
		{
			get
			{
				if (_stack.Count > 0) { return _stack.Peek(); }
				else { return default(IUIState<T>); }
			}
		}

		public void Push(IUIState<T> state)
		{
			DeactivateCurrent();
			_stack.Push(state);
			ActivateCurrent();
			state.Enter();
		}

		public IUIState<T> Pop()
		{
			if (_stack.Count > 0)
			{
				DeactivateCurrent();
				IUIState<T> state = _stack.Pop();
				state.Exit();

				if (hasState) { ActivateCurrent(); }
				return state;
			}
			else { return default(IUIState<T>); }
		}

		public void SwitchTo(IUIState<T> state)
		{
			PopAll();
			Push(state);
		}

		public void PopAll()
		{
			while (_stack.Count > 0) { Pop(); }
		}

        #endregion

        #region Private Properties

        private void ActivateCurrent()
		{
			if (hasState) { current.SetActive(); }
		}

		private void DeactivateCurrent()
		{
			if (hasState) { current.SetInactive(); }
        }

        #endregion
    }
}