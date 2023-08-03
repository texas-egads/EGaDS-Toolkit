using UnityEngine;
using egads.system.timer;

namespace egads.system.audio
{
    /// <summary>
    /// Plays a random sound with adjustable volume fading effect.
    /// </summary>
    public class RandomSoundVolume : MonoBehaviour
    {
        #region Public Properties

        /// <summary>
        /// The AudioSource used to play the random sound.
        /// </summary>
        public AudioSource source;

        /// <summary>
        /// The minimum interval time between volume changes.
        /// </summary>
        public float intervalMin = 5.0f;

        /// <summary>
        /// The maximum interval time between volume changes.
        /// </summary>
        public float intervalMax = 10.0f;

        /// <summary>
        /// If true, the sound will fade in and out between volume changes.
        /// </summary>
        public bool fadeInFadeOut = true;

        #endregion

        #region Private Properties

        private bool _active = true;
        private FadingTimer _timer;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            // If the AudioSource is not set, try to get it from the GameObject
            if (source == null) { source = GetComponent<AudioSource>(); }

            // Ensure that the AudioSource loops the sound and start playing it
            source.loop = true;
            if (!source.isPlaying) { source.Play(); }

            // Start the timer for the first volume change
            StartNewTimer();
        }

        private void Update()
        {
            // Check if the timer has ended (time for the next volume change)
            if (_timer.hasEnded)
            {
                _active = !_active;
                source.mute = !_active; // Mute or unmute the sound based on the _active flag
                StartNewTimer(); // Start a new timer for the next volume change
            }

            // If the sound is active (not muted), adjust its volume
            if (_active)
            {
                // If fadeInFadeOut is true, set the source volume based on the timer's progress (fading effect)
                // If fadeInFadeOut is false, set the source volume to 1 (no fading effect)
                if (fadeInFadeOut) { source.volume = _timer.progress; }
                else { source.volume = 1.0f; }
            }

            // Update the timer for volume changes
            _timer.Update();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Starts a new timer with fading effect between volume changes.
        /// </summary>
        private void StartNewTimer()
        {
            // Create a new FadingTimer with 20% fading in, random interval between intervalMin and intervalMax,
            // and 20% fading out.
            _timer = new FadingTimer(intervalMin * 0.2f, Random.Range(intervalMin, intervalMax), intervalMin * 0.2f);
        }

        #endregion
    }
}
