
namespace egads.system.input
{
	public interface IInputController
	{
        #region Input Scheme Properties

        InputScheme inputScheme { get; }
		event System.Action<InputScheme> inputSchemeChanged;

        #endregion
    }
}