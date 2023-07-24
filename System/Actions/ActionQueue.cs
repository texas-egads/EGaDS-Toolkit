using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace egads.system.actions
{
	/// <summary>
	/// List of actions that get executed one after another
	/// </summary>
	public class ActionQueue
	{
        #region Private Properties

        private Queue<IActionQueueElement> _queue = new Queue<IActionQueueElement>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Should be called every frame by parent object
        /// </summary>
        public void Update()
		{
			if (_queue.Count == 0)
				return;

			IActionQueueElement currentElement = _queue.Peek();
			currentElement.Update();

			if (currentElement.hasEnded)
			{
				currentElement.OnExit();
				_queue.Dequeue();

				if (_queue.Count > 0)
				{
					currentElement = _queue.Peek();
					currentElement.OnStart();
				}
			}
		}

		/// <summary>
		/// Adds a new element to the back of the queue
		/// </summary>
		/// <param name="element"></param>
		public void Add(IActionQueueElement element)
		{
			if (element != null)
			{
				_queue.Enqueue(element);

				if (_queue.Count == 1)
				{
					element.OnStart();
				}
			}
		}

		/// <summary>
		/// Adds a time duration where nothing happens
		/// </summary>
		/// <param name="duration"></param>
		public void AddPause(float duration)
		{
			Add(new ActionWait(duration));
		}

		/// <summary>
		/// Calls OnExit on the current element and clears all entries from the queue
		/// </summary>
		public void Clear()
		{
			if (_queue.Count > 0)
			{
				IActionQueueElement currentElement = _queue.Peek();
				currentElement.OnExit();

				_queue.Clear();
			}
		}

		public bool hasContent
		{
			get
			{
				return _queue.Count > 0;
			}
		}

        #endregion
    }
}