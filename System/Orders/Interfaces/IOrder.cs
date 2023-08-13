
namespace egads.system.orders
{
    /// <summary>
    /// Defines an order
    /// </summary>
    public interface IOrder
    {
        #region Methods

        /// <summary>
        /// Executes the order.
        /// </summary>
        void Execute();

        #endregion
    }
}
