﻿using UnityEngine;

namespace egads.system.characters
{
    /// <summary>
    /// Interface representing an animation controller for a character in the game.
    /// </summary>
    public interface IAnimationController
    {
        #region Methods

        /// <summary>
        /// Fades out the character's animation over a specified time.
        /// </summary>
        /// <param name="time">The duration of the fade-out animation in seconds (default is 1 second).</param>
        void FadeOut(float time = 1f);

        /// <summary>
        /// Initiates a fade-out animation for the character after their death.
        /// </summary>
        void FadeOutAfterDeath();

        /// <summary>
        /// Resets the animation controller to its initial state.
        /// </summary>
        void Reset();

        /// <summary>
        /// Sets the color of the character's material.
        /// </summary>
        /// <param name="color">The new color for the character's material.</param>
        void SetMaterialColor(Color color);

        #endregion
    }
}
