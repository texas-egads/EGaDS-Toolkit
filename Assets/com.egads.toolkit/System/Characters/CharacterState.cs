namespace egads.system.characters
{
    /// <summary>
    /// Represents the possible states an character can be in.
    /// </summary>
    public enum CharacterState
    {
        /// <summary>
        /// The character is idle and not performing any actions or movement.
        /// </summary>
        Idle,

        /// <summary>
        /// The character is in the process of moving towards a target.
        /// </summary>
        Moving,

        /// <summary>
        /// The character is currently taking order, such as attacking or performing a special ability.
        /// </summary>
        TakingOrder,

        /// <summary>
        /// The character is dead, which means it has zero health and is no longer active in the game.
        /// </summary>
        Dead,

        /// <summary>
        /// The character is disabled, usually due to external factors or temporary conditions that prevent it from taking any actions.
        /// </summary>
        Disabled
    }
}