using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace egads.system.orders
{
    /// <summary>
    /// Represents a list of orders that get executed all at the same time.
    /// </summary>
    public class OrderList
    {
        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether the order list has any content (orders).
        /// </summary>
        public bool hasContent => _list.Count > 0;

        #endregion

        #region Private Properties

        // List of order elements
        private List<IOrderQueueElement> _list = new List<IOrderQueueElement>();
        // Temporary list for all objects that have ended
        private List<IOrderQueueElement> _finishedList = new List<IOrderQueueElement>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Should be called every frame by the parent object to update all the orders in the list.
        /// </summary>
        public void Update()
        {
            // If the list is empty, no need to update anything
            if (_list.Count == 0) { return; }

            // Update all the orders in the list
            for (int i = 0; i < _list.Count; i++)
            {
                _list[i].Update();

                // Check if the order has ended and add it to the finishedList for cleanup
                if (_list[i].hasEnded) { _finishedList.Add(_list[i]); }
            }

            // Perform cleanup for all the orders that have ended
            for (int i = 0; i < _finishedList.Count; i++)
            {
                _finishedList[i].OnExit();
                _list.Remove(_finishedList[i]);
            }

            // Clear the finishedList to prepare it for the next frame
            _finishedList.Clear();
        }

        /// <summary>
        /// Adds a new element to the order list.
        /// </summary>
        /// <param name="element">The order to add.</param>
        public void Add(IOrderQueueElement element)
        {
            if (element != null)
            {
                _list.Add(element);

                // Call OnStart to perform setup for the new order.
                element.OnStart(); 
            }
        }

        /// <summary>
        /// Calls OnExit on all elements in the current list and clears all entries from the queue.
        /// </summary>
        public void Clear()
        {
            if (_list.Count > 0)
            {
                // Call OnExit for each order to perform cleanup.
                for (int i = 0; i < _list.Count; i++) { _list[i].OnExit(); }

                // Clear the list of orders.
                _list.Clear(); 
            }
        }

        #endregion
    }
}
