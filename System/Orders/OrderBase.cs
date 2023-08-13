using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace egads.system.orders
{
    /// <summary>
    /// This is a base class that implements the IOrderQueueElement interface.
    /// It provides default implementations for the interface methods.
    /// </summary>
    public class OrderBase : IOrderQueueElement
    {
        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether the order has ended.
        /// By default, this property always returns true, indicating that the order has ended.
        /// Derived classes can override this property to provide custom logic for determining the order's completion status.
        /// </summary>
        public virtual bool hasEnded => true;

        #endregion

        #region Public Methods

        /// <summary>
        /// Called when the order starts. This method can be overridden in derived classes to perform specific setup.
        /// </summary>
        public virtual void OnStart()
        {
            // empty: Default implementation does nothing.
        }

        /// <summary>
        /// Called each frame while the order is ongoing. This method can be overridden in derived classes to implement the order's logic.
        /// </summary>
        public virtual void Update()
        {
            // empty: Default implementation does nothing.
        }

        /// <summary>
        /// Called when the order is completed or interrupted. This method can be overridden in derived classes to perform cleanup or final orders.
        /// </summary>
        public virtual void OnExit()
        {
            // empty: Default implementation does nothing.
        }

        #endregion
    }
}
