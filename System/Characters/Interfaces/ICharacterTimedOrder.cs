namespace egads.system.characters
{
    /// <summary>
    /// Interface representing an chracter's timed order in the game.
    /// </summary>
    public interface ICharacterTimedOrder
    {
        #region Properties

        /// <summary>
        /// Gets the range of the timed order.
        /// </summary>
        float range { get; }

        /// <summary>
        /// Gets the cooldown time for the timed order.
        /// </summary>
        float cooldown { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Executes the timed action.
        /// </summary>
        void Execute();

        #endregion
    }
}
