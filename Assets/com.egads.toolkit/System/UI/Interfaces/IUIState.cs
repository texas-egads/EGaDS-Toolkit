using egads.system.input;

namespace egads.system.UI
{
    public interface IUIState<T> : INavigationInput
	{
        #region State

        T gameState { get; }

        #endregion

        #region Methods

        void Enter();
		void Exit();

		void SetActive();
		void SetInactive();

        #endregion
	}
}