using UnityEngine;

namespace egads.system.actors
{
    #region Delegates

    /// <summary>
    /// Delegate representing a method that will be called when the state of an actor changes.
    /// </summary>
    /// <param name="actor">The <see cref="IActor"/> instance whose state is changing.</param>
    /// <param name="state">The new state of the actor.</param>
    public delegate void StateChanged(IActor actor, ActorState state);

    #endregion

    /// <summary>
    /// Interface representing an actor in the game.
    /// </summary>
    public interface IActor
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether the actor is alive.
        /// </summary>
        bool isAlive { get; }

        /// <summary>
        /// Gets the 3D position of the actor in the game world.
        /// </summary>
        Vector3 position { get; }

        /// <summary>
        /// Gets the 2D position of the actor in the game world (ignores the Y-axis).
        /// </summary>
        Vector2 position2D { get; }

        /// <summary>
        /// Event that is raised when the state of the actor changes.
        /// </summary>
        event StateChanged stateChanged;

        #endregion
    }
}