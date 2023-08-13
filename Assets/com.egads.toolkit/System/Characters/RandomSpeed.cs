using UnityEngine;
using System;

namespace egads.system.characters
{
    /// <summary>
    /// Component that adds a random speed boost to an Character2D's movement speed upon Awake.
    /// </summary>
    public class RandomSpeed : MonoBehaviour
    {
        #region Public Properties

        /// <summary>
        /// The multiplier for the random speed boost. The random value will be in the range of (1 * multiplier) to (10 * multiplier).
        /// </summary>
        [Range(1f, 10f)]
        public float multiplier = 1f;

        #endregion

        #region Unity Methods

        /// <summary>
        /// Called when the GameObject is initialized.
        /// Adds a random speed boost to the Character2D's movement speed.
        /// </summary>
        private void Awake()
        {
            Character2D character = GetComponent<Character2D>();
            // Add a random value to the character's movement speed based on the multiplier.
            character.movementSpeed += UnityEngine.Random.Range(1f, multiplier);
        }

        #endregion
    }
}
