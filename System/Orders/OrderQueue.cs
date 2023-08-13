using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace egads.system.orders
{
    /// <summary>
    /// Represents a list of orders that get executed one after another (in a queue).
    /// </summary>
    public class OrderQueue
    {
        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether the order queue has any content (orders).
        /// </summary>
        public bool hasContent => _queue.Count > 0;

        #endregion

        #region Private Properties

        private Queue<IOrderQueueElement> _queue = new Queue<IOrderQueueElement>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Should be called every frame by the parent object to update the order queue.
        /// </summary>
        public void Update()
        {
            // If the queue is empty, no need to update anything
            if (_queue.Count == 0) { return; }

            // Get the current order in front of the queue
            IOrderQueueElement currentElement = _queue.Peek();

            // Update the current order
            currentElement.Update();

            // Check if the current order has ended
            if (currentElement.hasEnded)
            {
                // Perform OnExit on the current order
                currentElement.OnExit();

                // Remove the current order from the queue
                _queue.Dequeue();

                // If there are more orders in the queue, start the next order
                if (_queue.Count > 0)
                {
                    currentElement = _queue.Peek();
                    currentElement.OnStart();
                }
            }
        }

        /// <summary>
        /// Adds a new element to the back of the order queue.
        /// </summary>
        /// <param name="element">The order to add.</param>
        public void Add(IOrderQueueElement element)
        {
            if (element != null)
            {
                _queue.Enqueue(element);

                // If this is the first order in the queue, call its OnStart method.
                if (_queue.Count == 1) { element.OnStart(); }
            }
        }

        /// <summary>
        /// Adds a time duration where nothing happens (a pause) to the order queue.
        /// </summary>
        /// <param name="duration">The duration of the pause.</param>
        public void AddPause(float duration)
        {
            // Add an order to the queue that waits for the specified duration.
            Add(new OrderWait(duration));
        }

        /// <summary>
        /// Calls OnExit on the current element and clears all entries from the order queue.
        /// </summary>
        public void Clear()
        {
            if (_queue.Count > 0)
            {
                // Get the current order in front of the queue
                IOrderQueueElement currentElement = _queue.Peek();

                // Perform OnExit on the current order
                currentElement.OnExit();

                // Clear the queue
                _queue.Clear();
            }
        }

        #endregion
    }
}
