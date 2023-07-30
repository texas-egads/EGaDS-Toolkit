using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace egads.system.actions
{
    /// <summary>
    /// This is a base class that implements the IActionQueueElement interface.
    /// It provides default implementations for the interface methods.
    /// </summary>
    public class ActionBase : IActionQueueElement
    {
        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether the action has ended.
        /// By default, this property always returns true, indicating that the action has ended.
        /// Derived classes can override this property to provide custom logic for determining the action's completion status.
        /// </summary>
        public virtual bool hasEnded => true;

        #endregion

        #region Public Methods

        /// <summary>
        /// Called when the action starts. This method can be overridden in derived classes to perform specific setup.
        /// </summary>
        public virtual void OnStart()
        {
            // empty: Default implementation does nothing.
        }

        /// <summary>
        /// Called each frame while the action is ongoing. This method can be overridden in derived classes to implement the action's logic.
        /// </summary>
        public virtual void Update()
        {
            // empty: Default implementation does nothing.
        }

        /// <summary>
        /// Called when the action is completed or interrupted. This method can be overridden in derived classes to perform cleanup or final actions.
        /// </summary>
        public virtual void OnExit()
        {
            // empty: Default implementation does nothing.
        }

        #endregion
    }
}
