using egads.tools.extensions;
using System.Collections.Generic;
using UnityEngine;

namespace egads.system.characters
{
    /// <summary>
    /// Component that detects and tracks other characters within its 2D trigger collider and raises events for detected and lost characters.
    /// The detected characters are filtered based on their tags (if provided).
    /// This class uses the "ICharacter" interface for character representation and provides a basic event system for handling character detection and state changes.
    /// </summary>
    public class Sensor2D : MonoBehaviour
    {
        #region Sensor Events

        /// <summary>
        /// Delegate for sensor events.
        /// </summary>
        /// <param name="type">The type of sensor event.</param>
        /// <param name="character">The detected or lost character.</param>
        public delegate void SensorEventDelegate(SensorEvent type, ICharacter character);
        public event SensorEventDelegate sensorEvent;

        #endregion

        #region Public Properties

        /// <summary>
        /// The tag to filter detected characters. If empty, all characters are detected.
        /// </summary>
        public string searchTag = "";

        /// <summary>
        /// List of characters detected by the sensor.
        /// </summary>
        public List<ICharacter> characters = new List<ICharacter>();

        /// <summary>
        /// Gets a value indicating whether the sensor has detected any characters.
        /// </summary>
        public bool hasCharactersDetected => characters.Count > 0;

        #endregion

        #region Private Properties

        protected Transform _transform;
        protected ICharacter _self;

        #endregion

        #region Unity Methods

        /// <summary>
        /// Called when the GameObject is initialized.
        /// </summary>
        private void Awake()
        {
            _transform = transform;

            _self = _transform.GetInterface<ICharacter>();
            if (_self == null && _transform.parent != null)
            {
                _self = _transform.parent.GetInterface<ICharacter>();
            }
        }

        /// <summary>
        /// Called when a GameObject enters the 2D trigger collider.
        /// Detects characters and raises events accordingly.
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (searchTag == "" || other.tag == searchTag)
            {
                ICharacter character = other.transform.GetInterface<ICharacter>();
                if (character != null && character != _self && !characters.Contains(character) && character.isAlive)
                {
                    characters.Add(character);

                    // Subscribe to the character's stateChanged event to handle changes in character state.
                    character.stateChanged += Character_StateChanged;

                    // Raise the sensor event for character detection.
                    if (sensorEvent != null) { sensorEvent(SensorEvent.CharacterDetected, character); }
                }
            }
        }

        /// <summary>
        /// Called when a GameObject exits the 2D trigger collider.
        /// Removes characters from the detected list and raises events accordingly.
        /// </summary>
        private void OnTriggerExit2D(Collider2D other)
        {
            if (searchTag == "" || other.tag == searchTag)
            {
                ICharacter character = other.transform.GetInterface<ICharacter>();
                Remove(character);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Event handler for the character's stateChanged event.
        /// Handles the removal of characters when they become inactive (dead).
        /// </summary>
        private void Character_StateChanged(ICharacter character, CharacterState state)
        {
            if (!character.isAlive)
            {
                if (characters.Contains(character))
                {
                    Remove(character);
                }
                else
                {
                    // Unsubscribe from the character's stateChanged event if the character is no longer in the detected list.
                    character.stateChanged -= Character_StateChanged;
                }
            }
        }

        /// <summary>
        /// Removes the specified character from the detected list and raises the sensor event accordingly.
        /// </summary>
        private void Remove(ICharacter character)
        {
            if (character != null && characters.Contains(character))
            {
                characters.Remove(character);

                // Unsubscribe from the character's stateChanged event.
                character.stateChanged -= Character_StateChanged;

                // Raise the sensor event for character leaving.
                if (sensorEvent != null)
                    sensorEvent(SensorEvent.CharacterLeft, character);
            }
        }

        /// <summary>
        /// Updates the detected characters list by removing any null references.
        /// </summary>
        private void UpdateList()
        {
            for (int i = 0; i < characters.Count; i++)
            {
                if (characters[i] == null)
                {
                    characters.RemoveAt(i);
                    i--;
                }
            }
        }

        #endregion
    }
}