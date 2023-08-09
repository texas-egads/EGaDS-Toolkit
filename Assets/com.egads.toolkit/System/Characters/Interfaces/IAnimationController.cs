using UnityEngine;

namespace egads.system.characters
{
    /// <summary>
    /// Interface representing an animation controller for an actor in the game.
    /// </summary>
    public interface IAnimationController
    {
        #region Methods

        /// <summary>
        /// Fades out the actor's animation over a specified time.
        /// </summary>
        /// <param name="time">The duration of the fade-out animation in seconds (default is 1 second).</param>
        void FadeOut(float time = 1f);

        /// <summary>
        /// Initiates a fade-out animation for the actor after their death.
        /// </summary>
        void FadeOutAfterDeath();

        /// <summary>
        /// Resets the animation controller to its initial state.
        /// </summary>
        void Reset();

        /// <summary>
        /// Sets the color of the actor's material.
        /// </summary>
        /// <param name="color">The new color for the actor's material.</param>
        void SetMaterialColor(Color color);

        #endregion
    }
}
