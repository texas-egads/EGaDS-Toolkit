using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace egads.system.actions
{
	/// <summary>
	/// Describes an action that lasts a certain duration and can be added to a chronological queue
	/// </summary>
	public interface IActionQueueElement
	{
        #region Properties

        /// <summary>
        /// Gets a value indicating whether the action has ended.
        /// </summary>
        bool hasEnded { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Called when the action starts. Perform any necessary setup here.
        /// </summary>
        void OnStart();

        /// <summary>
        /// Called each frame while the action is ongoing. Implement the main logic of the action here.
        /// </summary>
        void Update();

        /// <summary>
        /// Called when the action is completed or interrupted. Clean up resources or perform final actions here.
        /// </summary>
        void OnExit();

        #endregion
    }
}