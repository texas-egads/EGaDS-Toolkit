namespace egads.system.characters
{
    /// <summary>
    /// Represents the possible states an actor can be in.
    /// </summary>
    public enum CharacterState
    {
        /// <summary>
        /// The actor is idle and not performing any actions or movement.
        /// </summary>
        Idle,

        /// <summary>
        /// The actor is in the process of moving towards a target.
        /// </summary>
        Moving,

        /// <summary>
        /// The actor is currently taking action, such as attacking or performing a special ability.
        /// </summary>
        TakingAction,

        /// <summary>
        /// The actor is dead, which means it has zero health and is no longer active in the game.
        /// </summary>
        Dead,

        /// <summary>
        /// The actor is disabled, usually due to external factors or temporary conditions that prevent it from taking any actions.
        /// </summary>
        Disabled
    }
}