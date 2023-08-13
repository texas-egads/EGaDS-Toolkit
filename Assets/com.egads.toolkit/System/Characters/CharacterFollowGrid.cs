using UnityEngine;
using egads.system.pathFinding;

namespace egads.system.characters
{
    /// <summary>
    /// A class that enables an character to follow a grid-based path.
    /// </summary>
    public class CharacterFollowGrid : MonoBehaviour
    {
        #region Unity Methods

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Initializes the character's target with a path field from the level grid.
        /// </summary>
        private void Start()
        {
            // Get the Character2D component attached to this GameObject
            Character2D character = GetComponent<Character2D>();

            // Find the LevelGrid component in the scene and set it as the path field for the character's target
            character.target.SetPathField(FindObjectOfType<LevelGrid>());
        }

        #endregion
    }
}
