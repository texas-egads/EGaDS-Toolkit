using UnityEngine;

namespace egads.system.characters
{
    /// <summary>
    /// Component that updates the rendering depth of a GameObject based on its position in the scene.
    /// </summary>
    public class RenderDepth : MonoBehaviour
    {
        #region Public Properties

        /// <summary>
        /// If true, move the GameObject to the background when it becomes dead (inactive).
        /// </summary>
        public bool toBackgroundWhenDead = false;

        /// <summary>
        /// The offset value to apply to the rendering depth.
        /// </summary>
        public float offset = 0f;

        #endregion

        #region Private Properties

        // State
        private bool _isActive = true;

        private Transform _transform;
        private Character2D _character;

        #endregion

        #region Unity Methods

        /// <summary>
        /// Called when the GameObject is initialized.
        /// </summary>
        private void Awake()
        {
            _transform = transform;

            _character = GetComponent<Character2D>();
            if (_character != null)
            {
                // Subscribe to the character's stateChanged event to handle changes in character state.
                _character.stateChanged += CharacterStateChanged;
            }
        }

        /// <summary>
        /// Called once per frame.
        /// Update the rendering depth of the GameObject based on its position in the scene.
        /// </summary>
        private void Update()
        {
            if (_isActive)
            {
                // Update the GameObject's position in the scene to adjust its rendering depth.
                _transform.position = new Vector3(_transform.position.x, _transform.position.y, _transform.position.y * 0.1f + offset);
            }
        }

        /// <summary>
        /// Updates the position of the GameObject with the given current position.
        /// </summary>
        /// <param name="current">The current position of the GameObject.</param>
        /// <returns>The updated position with adjusted rendering depth.</returns>
        public Vector3 UpdatePosition(Vector3 current) => new Vector3(current.x, current.y, current.y * 0.1f + offset);

        #endregion

        #region Private Methods

        /// <summary>
        /// Event handler for the character's stateChanged event.
        /// Resets the rendering depth when the character becomes active again after being dead (inactive).
        /// </summary>
        /// <param name="activeCharacter">The character that changed state.</param>
        /// <param name="state">The new state of the character.</param>
        private void CharacterStateChanged(ICharacter activeCharacter, CharacterState state)
        {
            // Reset depth rendering when character is active again after being dead.
            if (activeCharacter.isAlive && !_isActive)
            {
                _isActive = true;
            }
        }

        #endregion
    }
}