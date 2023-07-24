
namespace egads.system.stateMachine
{
	public interface IState
	{
        #region Methods

        void OnEnter();
		void OnUpdate();
		void OnExit();
		void OnGotFocus();
		void OnLostFocus();

        #endregion
    }
}