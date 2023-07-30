using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace egads.system.actions
{
    /// <summary>
    /// Represents a list of actions that get executed one after another (in a queue).
    /// </summary>
    public class ActionQueue
    {
        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether the action queue has any content (actions).
        /// </summary>
        public bool hasContent => _queue.Count > 0;

        #endregion

        #region Private Properties

        private Queue<IActionQueueElement> _queue = new Queue<IActionQueueElement>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Should be called every frame by the parent object to update the action queue.
        /// </summary>
        public void Update()
        {
            // If the queue is empty, no need to update anything
            if (_queue.Count == 0) { return; }

            // Get the current action in front of the queue
            IActionQueueElement currentElement = _queue.Peek();

            // Update the current action
            currentElement.Update();

            // Check if the current action has ended
            if (currentElement.hasEnded)
            {
                // Perform OnExit on the current action
                currentElement.OnExit();

                // Remove the current action from the queue
                _queue.Dequeue();

                // If there are more actions in the queue, start the next action
                if (_queue.Count > 0)
                {
                    currentElement = _queue.Peek();
                    currentElement.OnStart();
                }
            }
        }

        /// <summary>
        /// Adds a new element to the back of the action queue.
        /// </summary>
        /// <param name="element">The action to add.</param>
        public void Add(IActionQueueElement element)
        {
            if (element != null)
            {
                _queue.Enqueue(element);

                // If this is the first action in the queue, call its OnStart method.
                if (_queue.Count == 1) { element.OnStart(); }
            }
        }

        /// <summary>
        /// Adds a time duration where nothing happens (a pause) to the action queue.
        /// </summary>
        /// <param name="duration">The duration of the pause.</param>
        public void AddPause(float duration)
        {
            // Add an action to the queue that waits for the specified duration.
            Add(new ActionWait(duration));
        }

        /// <summary>
        /// Calls OnExit on the current element and clears all entries from the action queue.
        /// </summary>
        public void Clear()
        {
            if (_queue.Count > 0)
            {
                // Get the current action in front of the queue
                IActionQueueElement currentElement = _queue.Peek();

                // Perform OnExit on the current action
                currentElement.OnExit();

                // Clear the queue
                _queue.Clear();
            }
        }

        #endregion
    }
}
