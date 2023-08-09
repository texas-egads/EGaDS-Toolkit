using UnityEngine;
using System.Collections.Generic;

namespace egads.system.actors
{
    /// <summary>
    /// Component that detects and tracks other actors within its 2D trigger collider and raises events for detected and lost actors.
    /// The detected actors are filtered based on their tags (only actors with a specified tag are detected).
    /// This class represents actors using the "Actor2D" class and provides public methods to get the most wounded and nearest actors from the detected list.
    /// </summary>
    public class Sensors : MonoBehaviour
    {
        #region Sensor Events

        /// <summary>
        /// Delegate for sensor events.
        /// </summary>
        /// <param name="type">The type of sensor event.</param>
        /// <param name="otherActor">The detected or lost actor.</param>
        public delegate void SensorEventDelegate(SensorEvent type, Character2D otherActor);
        public event SensorEventDelegate sensorEvent;

        #endregion

        #region Public Properties

        /// <summary>
        /// The tag to filter detected actors. Only actors with this tag will be detected.
        /// </summary>
        public string Tag = "Player";

        /// <summary>
        /// List of actors detected by the sensor.
        /// </summary>
        public List<Character2D> actors = new List<Character2D>();

        /// <summary>
        /// Gets a value indicating whether the sensor has detected any actors.
        /// </summary>
        public bool ActorsDetected => actors.Count > 0;

        #endregion

        #region Private Properties

        private Transform _transform;

        #endregion

        #region Unity Methods

        /// <summary>
        /// Called when the GameObject is initialized.
        /// </summary>
        private void Awake()
        {
            _transform = transform;
        }

        /// <summary>
        /// Called when a GameObject enters the 2D trigger collider.
        /// Detects actors and raises events accordingly.
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == Tag)
            {
                Character2D actor = other.GetComponent<Character2D>();
                if (actor != null && !actors.Contains(actor) && actor.isAlive)
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
            if (other.tag == Tag)
            {
                Character2D actor = other.GetComponent<Character2D>();
                RemoveActor(actor);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the most wounded actor from the detected actors list.
        /// </summary>
        /// <returns>The most wounded actor, or null if no actors are detected.</returns>
        public Character2D GetMostWoundedActor()
        {
            UpdateList();

            if (actors.Count == 0) { return null; }

            Character2D targetActor = actors[0];
            float woundAmount = targetActor.health.missingAmount;

            for (int i = 1; i < actors.Count; i++)
            {
                float newHealthAmount = actors[i].health.missingAmount;
                if (newHealthAmount > woundAmount)
                {
                    woundAmount = newHealthAmount;
                    targetActor = actors[i];
                }
            }

            return targetActor;
        }

        /// <summary>
        /// Gets the nearest actor from the detected actors list.
        /// </summary>
        /// <returns>The nearest actor, or null if no actors are detected.</returns>
        public Character2D GetNearestActor()
        {
            UpdateList();

            if (actors.Count == 0) { return null; }

            Vector2 position = _transform.position;
            Character2D nearestActor = actors[0];
            float foundDistance = (position - nearestActor.position2D).sqrMagnitude;

            for (int i = 1; i < actors.Count; i++)
            {
                float newDistance = (position - actors[i].position2D).sqrMagnitude;
                if (newDistance < foundDistance)
                {
                    foundDistance = newDistance;
                    nearestActor = actors[i];
                }
            }

            return nearestActor;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles changes in actor state and removes dead actors from the detected list.
        /// </summary>
        private void Actor_StateChanged(ICharacter actor, CharacterState state)
        {
            if (!actor.isAlive)
            {
                if (actor is Character2D actor2D) { RemoveActor(actor2D); }
                else { actor.stateChanged -= Actor_StateChanged; }
            }
        }

        /// <summary>
        /// Removes the specified actor from the detected list and raises the sensor event accordingly.
        /// </summary>
        private void RemoveActor(Character2D actor)
        {
            if (actor != null && actors.Contains(actor))
            {
                actors.Remove(actor);

                // Unsubscribe from the actor's stateChanged event.
                actor.stateChanged -= Actor_StateChanged;

                // Raise the sensor event for actor leaving.
                if (sensorEvent != null) { sensorEvent(SensorEvent.ActorLeft, actor); }
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
