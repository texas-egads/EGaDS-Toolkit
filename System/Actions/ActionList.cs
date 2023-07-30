using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace egads.system.actions
{
    /// <summary>
    /// Represents a list of actions that get executed all at the same time.
    /// </summary>
    public class ActionList
    {
        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether the action list has any content (actions).
        /// </summary>
        public bool hasContent => _list.Count > 0;

        #endregion

        #region Private Properties

        // List of action elements
        private List<IActionQueueElement> _list = new List<IActionQueueElement>();
        // Temporary list for all objects that have ended
        private List<IActionQueueElement> _finishedList = new List<IActionQueueElement>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Should be called every frame by the parent object to update all the actions in the list.
        /// </summary>
        public void Update()
        {
            // If the list is empty, no need to update anything
            if (_list.Count == 0) { return; }

            // Update all the actions in the list
            for (int i = 0; i < _list.Count; i++)
            {
                _list[i].Update();

                // Check if the action has ended and add it to the finishedList for cleanup
                if (_list[i].hasEnded) { _finishedList.Add(_list[i]); }
            }

            // Perform cleanup for all the actions that have ended
            for (int i = 0; i < _finishedList.Count; i++)
            {
                _finishedList[i].OnExit();
                _list.Remove(_finishedList[i]);
            }

            // Clear the finishedList to prepare it for the next frame
            _finishedList.Clear();
        }

        /// <summary>
        /// Adds a new element to the action list.
        /// </summary>
        /// <param name="element">The action to add.</param>
        public void Add(IActionQueueElement element)
        {
            if (element != null)
            {
                _list.Add(element);

                // Call OnStart to perform setup for the new action.
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
                // Call OnExit for each action to perform cleanup.
                for (int i = 0; i < _list.Count; i++) { _list[i].OnExit(); }

                // Clear the list of actions.
                _list.Clear(); 
            }
        }

        #endregion
    }
}
