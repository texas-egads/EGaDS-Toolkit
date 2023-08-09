using UnityEngine;
using egads.system.pathFinding;

namespace egads.system.actors
{
    /// <summary>
    /// A class that enables an actor to follow a grid-based path.
    /// </summary>
    public class CharacterFollowGrid : MonoBehaviour
    {
        #region Unity Methods

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Initializes the actor's target with a path field from the level grid.
        /// </summary>
        private void Start()
        {
            // Get the Actor2D component attached to this GameObject
            Character2D actor = GetComponent<Character2D>();

            // Find the LevelGrid component in the scene and set it as the path field for the actor's target
            actor.target.SetPathField(FindObjectOfType<LevelGrid>());
        }

        #endregion
    }
}
