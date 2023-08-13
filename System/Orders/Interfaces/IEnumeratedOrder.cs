using System.Collections;

namespace egads.system.orders
{
    /// <summary>
    /// Defines and order with an enumerated execute method
    /// </summary>
    public interface IEnumeratedOrder
    {
        #region Methods

        /// <summary>
        /// This method represents the order's execution process as an IEnumerator.
        /// It allows the order to be executed in multiple steps and supports iteration.
        /// </summary>
        /// <returns>An IEnumerator representing the execution process of the order.</returns>
        IEnumerator Execute();

        #endregion
    }
}