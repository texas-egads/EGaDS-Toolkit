using UnityEngine;
using egads.system.actors;

namespace egads.system.audio
{
    /// <summary>
    /// Plays stepping sounds when the associated Actor2D is moving.
    /// </summary>
    public class SteppingSound : MonoBehaviour
    {
        #region Public Properties

        /// <summary>
        /// The AudioSource used to play the stepping sounds.
        /// </summary>
        public AudioSource source;

        #endregion

        #region Private Properties

        private Character2D _actor;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            // If the AudioSource is not set, try to get it from the GameObject
            if (source == null) { source = GetComponent<AudioSource>(); }

            // Get the associated Actor2D component
            _actor = GetComponent<Character2D>();

            // Configure the AudioSource properties
            source.loop = true;
            source.Stop();
        }

        private void Update()
        {
            // Check if the associated Actor2D is moving
            if (_actor.isMoving)
            {
                // If the Actor is moving and the sound is not playing, start playing it
                if (!source.isPlaying) { source.Play(); }
            }
            else
            {
                // If the Actor is not moving and the sound is playing, stop it
                if (source.isPlaying) { source.Stop(); }
            }
        }

        #endregion
    }
}