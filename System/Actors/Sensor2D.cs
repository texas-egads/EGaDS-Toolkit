﻿using egads.tools.extensions;
using System.Collections.Generic;
using UnityEngine;

namespace egads.system.actors
{
    /// <summary>
    /// Component that detects and tracks other actors within its 2D trigger collider and raises events for detected and lost actors.
    /// The detected actors are filtered based on their tags (if provided).
    /// This class uses the "IActor" interface for actor representation and provides a basic event system for handling actor detection and state changes.
    /// </summary>
    public class Sensor2D : MonoBehaviour
    {
        #region Sensor Events

        /// <summary>
        /// Delegate for sensor events.
        /// </summary>
        /// <param name="type">The type of sensor event.</param>
        /// <param name="actor">The detected or lost actor.</param>
        public delegate void SensorEventDelegate(SensorEvent type, IActor actor);
        public event SensorEventDelegate sensorEvent;

        #endregion

        #region Public Properties

        /// <summary>
        /// The tag to filter detected actors. If empty, all actors are detected.
        /// </summary>
        public string searchTag = "";

        /// <summary>
        /// List of actors detected by the sensor.
        /// </summary>
        public List<IActor> actors = new List<IActor>();

        /// <summary>
        /// Gets a value indicating whether the sensor has detected any actors.
        /// </summary>
        public bool hasActorsDetected => actors.Count > 0;

        #endregion

        #region Private Properties

        protected Transform _transform;
        protected IActor _self;

        #endregion

        #region Unity Methods

        /// <summary>
        /// Called when the GameObject is initialized.
        /// </summary>
        private void Awake()
        {
            _transform = transform;

            _self = _transform.GetInterface<IActor>();
            if (_self == null && _transform.parent != null)
            {
                _self = _transform.parent.GetInterface<IActor>();
            }
        }

        /// <summary>
        /// Called when a GameObject enters the 2D trigger collider.
        /// Detects actors and raises events accordingly.
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (searchTag == "" || other.tag == searchTag)
            {
                IActor actor = other.transform.GetInterface<IActor>();
                if (actor != null && actor != _self && !actors.Contains(actor) && actor.isAlive)
                {
                    actors.Add(actor);

                    // Subscribe to the actor's stateChanged event to handle changes in actor state.
                    actor.stateChanged += Actor_StateChanged;

                    // Raise the sensor event for actor detection.
                    if (sensorEvent != null) { sensorEvent(SensorEvent.ActorDetected, actor); }
                }
            }
        }

        /// <summary>
        /// Called when a GameObject exits the 2D trigger collider.
        /// Removes actors from the detected list and raises events accordingly.
        /// </summary>
        private void OnTriggerExit2D(Collider2D other)
        {
            if (searchTag == "" || other.tag == searchTag)
            {
                IActor actor = other.transform.GetInterface<IActor>();
                Remove(actor);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Event handler for the actor's stateChanged event.
        /// Handles the removal of actors when they become inactive (dead).
        /// </summary>
        private void Actor_StateChanged(IActor actor, ActorState state)
        {
            if (!actor.isAlive)
            {
                if (actors.Contains(actor))
                {
                    Remove(actor);
                }
                else
                {
                    // Unsubscribe from the actor's stateChanged event if the actor is no longer in the detected list.
                    actor.stateChanged -= Actor_StateChanged;
                }
            }
        }

        /// <summary>
        /// Removes the specified actor from the detected list and raises the sensor event accordingly.
        /// </summary>
        private void Remove(IActor actor)
        {
            if (actor != null && actors.Contains(actor))
            {
                actors.Remove(actor);

                // Unsubscribe from the actor's stateChanged event.
                actor.stateChanged -= Actor_StateChanged;

                // Raise the sensor event for actor leaving.
                if (sensorEvent != null)
                    sensorEvent(SensorEvent.ActorLeft, actor);
            }
        }

        /// <summary>
        /// Updates the detected actors list by removing any null references.
        /// </summary>
        private void UpdateList()
        {
            for (int i = 0; i < actors.Count; i++)
            {
                if (actors[i] == null)
                {
                    actors.RemoveAt(i);
                    i--;
                }
            }
        }

        #endregion
    }
}
