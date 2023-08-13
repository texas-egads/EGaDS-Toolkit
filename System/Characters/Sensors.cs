using UnityEngine;
using System.Collections.Generic;

namespace egads.system.characters
{
    /// <summary>
    /// Component that detects and tracks other characters within its 2D trigger collider and raises events for detected and lost characters.
    /// The detected characters are filtered based on their tags (only characters with a specified tag are detected).
    /// This class represents characters using the "Character2D" class and provides public methods to get the most wounded and nearest characters from the detected list.
    /// </summary>
    public class Sensors : MonoBehaviour
    {
        #region Sensor Events

        /// <summary>
        /// Delegate for sensor events.
        /// </summary>
        /// <param name="type">The type of sensor event.</param>
        /// <param name="otherCharacter">The detected or lost character.</param>
        public delegate void SensorEventDelegate(SensorEvent type, Character2D otherCharacter);
        public event SensorEventDelegate sensorEvent;

        #endregion

        #region Public Properties

        /// <summary>
        /// The tag to filter detected characters. Only characters with this tag will be detected.
        /// </summary>
        public string Tag = "Player";

        /// <summary>
        /// List of characters detected by the sensor.
        /// </summary>
        public List<Character2D> characters = new List<Character2D>();

        /// <summary>
        /// Gets a value indicating whether the sensor has detected any characters.
        /// </summary>
        public bool hasCharactersDetected => characters.Count > 0;

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
        /// Detects characters and raises events accordingly.
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == Tag)
            {
                Character2D character = other.GetComponent<Character2D>();
                if (character != null && !characters.Contains(character) && character.isAlive)
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
            if (other.tag == Tag)
            {
                Character2D character = other.GetComponent<Character2D>();
                RemoveCharacter(character);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the most wounded character from the detected characters list.
        /// </summary>
        /// <returns>The most wounded character, or null if no characters are detected.</returns>
        public Character2D GetMostWoundedCharacter()
        {
            UpdateList();

            if (characters.Count == 0) { return null; }

            Character2D targetCharacter = characters[0];
            float woundAmount = targetCharacter.health.missingAmount;

            for (int i = 1; i < characters.Count; i++)
            {
                float newHealthAmount = characters[i].health.missingAmount;
                if (newHealthAmount > woundAmount)
                {
                    woundAmount = newHealthAmount;
                    targetCharacter = characters[i];
                }
            }

            return targetCharacter;
        }

        /// <summary>
        /// Gets the nearest character from the detected characters list.
        /// </summary>
        /// <returns>The nearest character, or null if no characters are detected.</returns>
        public Character2D GetNearestCharacter()
        {
            UpdateList();

            if (characters.Count == 0) { return null; }

            Vector2 position = _transform.position;
            Character2D nearestCharacter = characters[0];
            float foundDistance = (position - nearestCharacter.position2D).sqrMagnitude;

            for (int i = 1; i < characters.Count; i++)
            {
                float newDistance = (position - characters[i].position2D).sqrMagnitude;
                if (newDistance < foundDistance)
                {
                    foundDistance = newDistance;
                    nearestCharacter = characters[i];
                }
            }

            return nearestCharacter;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles changes in character state and removes dead characters from the detected list.
        /// </summary>
        private void Character_StateChanged(ICharacter character, CharacterState state)
        {
            if (!character.isAlive)
            {
                if (character is Character2D character2D) { RemoveCharacter(character2D); }
                else { character.stateChanged -= Character_StateChanged; }
            }
        }

        /// <summary>
        /// Removes the specified character from the detected list and raises the sensor event accordingly.
        /// </summary>
        private void RemoveCharacter(Character2D character)
        {
            if (character != null && characters.Contains(character))
            {
                characters.Remove(character);

                // Unsubscribe from the character's stateChanged event.
                character.stateChanged -= Character_StateChanged;

                // Raise the sensor event for character leaving.
                if (sensorEvent != null) { sensorEvent(SensorEvent.CharacterLeft, character); }
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
