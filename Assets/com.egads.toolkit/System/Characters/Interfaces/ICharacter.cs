using UnityEngine;

namespace egads.system.characters
{
    #region Delegates

    /// <summary>
    /// Delegate representing a method that will be called when the state of an character changes.
    /// </summary>
    /// <param name="character">The <see cref="ICharacter"/> instance whose state is changing.</param>
    /// <param name="state">The new state of the character.</param>
    public delegate void StateChanged(ICharacter character, CharacterState state);

    #endregion

    /// <summary>
    /// Interface representing an character in the game.
    /// </summary>
    public interface ICharacter
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether the character is alive.
        /// </summary>
        bool isAlive { get; }

        /// <summary>
        /// Gets the 3D position of the character in the game world.
        /// </summary>
        Vector3 position { get; }

        /// <summary>
        /// Gets the 2D position of the character in the game world (ignores the Y-axis).
        /// </summary>
        Vector2 position2D { get; }

        /// <summary>
        /// Event that is raised when the state of the character changes.
        /// </summary>
        event StateChanged stateChanged;

        #endregion
    }
}