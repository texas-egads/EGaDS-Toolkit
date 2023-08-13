using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace egads.system.orders
{
    /// <summary>
    /// Describes an order that lasts a certain duration and can be added to a chronological queue
    /// </summary>
    public interface IOrderQueueElement
	{
        #region Properties

        /// <summary>
        /// Gets a value indicating whether the order has ended.
        /// </summary>
        bool hasEnded { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Called when the order starts. Perform any necessary setup here.
        /// </summary>
        void OnStart();

        /// <summary>
        /// Called each frame while the order is ongoing. Implement the main logic of the order here.
        /// </summary>
        void Update();

        /// <summary>
        /// Called when the order is completed or interrupted. Clean up resources or perform final orders here.
        /// </summary>
        void OnExit();

        #endregion
    }
}