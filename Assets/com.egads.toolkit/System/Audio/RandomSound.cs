using UnityEngine;
using egads.system.timer;

namespace egads.system.audio
{
    /// <summary>
    /// Plays random sounds with configurable pause intervals.
    /// </summary>
    public class RandomSound : MonoBehaviour
    {
        #region Public Properties

        /// <summary>
        /// The AudioSource used to play the random sounds.
        /// </summary>
        public AudioSource source;

        /// <summary>
        /// The minimum interval time between sounds.
        /// </summary>
        public float pauseIntervalMin;

        /// <summary>
        /// The maximum interval time between sounds.
        /// </summary>
        public float pauseIntervalMax;

        #endregion

        #region Private Properties

        private Timer _timer = null;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            // If the AudioSource is not set, try to get it from the GameObject
            if (source == null) { source = GetComponent<AudioSource>(); }

            // Ensure that the AudioSource does not loop the sound
            if (source != null) { source.loop = false; }

            // Start the timer to play the first sound
            StartNewTimer();
        }

        private void Update()
        {
            // Check if the AudioSource is not playing
            if (source != null && !source.isPlaying)
            {
                // If the timer is null, start a new one; otherwise, update the existing timer
                if (_timer == null) { StartNewTimer(); }
                else { _timer.Update(); }

                // Check if the timer has ended (time for the next sound)
                if (_timer.hasEnded)
                {
                    _timer = null;
                    source.Play(); // Play the sound
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Starts a new timer with a random interval between pauseIntervalMin and pauseIntervalMax.
        /// </summary>
        private void StartNewTimer()
        {
            _timer = new Timer(Random.Range(pauseIntervalMin, pauseIntervalMax));
        }

        #endregion
    }
}
