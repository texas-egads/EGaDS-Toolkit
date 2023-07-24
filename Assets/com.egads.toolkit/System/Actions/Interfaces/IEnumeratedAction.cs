using System.Collections;

namespace egads.system.actions
{
    /// <summary>
    /// Defines and action with an enumerated execute method
    /// </summary>
    public interface IEnumeratedAction
    {
        #region Methods

        IEnumerator Execute();

        #endregion
    }
}