using System.Collections;

namespace egads.system.actions
{
    /// <summary>
    /// Defines and action with an enumerated execute method
    /// </summary>
    public interface IEnumeratedAction
    {
        #region Methods

        /// <summary>
        /// This method represents the action's execution process as an IEnumerator.
        /// It allows the action to be executed in multiple steps and supports iteration.
        /// </summary>
        /// <returns>An IEnumerator representing the execution process of the action.</returns>
        IEnumerator Execute();

        #endregion
    }
}