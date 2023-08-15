namespace egads.system.gameManagement
{
    #region Game State

    /// <summary>
    /// Represents the possible states of the game.
    /// </summary>
    public enum GameState
    {
        /// <summary>
        /// The game is currently running and active.
        /// </summary>
        Running,

        /// <summary>
        /// The game is paused, typically due to user input or interruption.
        /// </summary>
        Paused,

        /// <summary>
        /// The game is in a menu or UI state, allowing user interactions.
        /// </summary>
        Menu,

        /// <summary>
        /// The game has ended or reached a conclusion.
        /// </summary>
        Ended,

        /// <summary>
        /// The game is in a loading state, often when transitioning scenes or levels.
        /// </summary>
        Loading,

        /// <summary>
        /// The game is in a sequence or cinematic state.
        /// </summary>
        Sequence,

        /// <summary>
        /// No specific game state is defined or applicable.
        /// </summary>
        None
    }

    #endregion
}
