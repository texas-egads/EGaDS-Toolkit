namespace egads.system.characters
{
    /// <summary>
    /// Interface representing an entity that has health in the game.
    /// </summary>
    public interface IHasHealth
    {
        #region Properties

        /// <summary>
        /// Gets or sets the current health of the entity.
        /// </summary>
        Energy health { get; set; }

        #endregion
    }
}
