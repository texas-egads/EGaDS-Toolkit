namespace egads.system.actors
{
    /// <summary>
    /// Interface representing an actor's timed action in the game.
    /// </summary>
    public interface ICharacterTimedAction
    {
        #region Properties

        /// <summary>
        /// Gets the range of the timed action.
        /// </summary>
        float range { get; }

        /// <summary>
        /// Gets the cooldown time for the timed action.
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
