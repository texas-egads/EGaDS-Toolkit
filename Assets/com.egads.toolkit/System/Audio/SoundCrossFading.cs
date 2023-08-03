using UnityEngine;
using System.Collections.Generic;
using egads.tools.extensions;

namespace egads.system.audio
{
    /// <summary>
    /// Crossfades between multiple audio clips for smoother transitions.
    /// </summary>
    public class SoundCrossFading : MonoBehaviour
    {
        #region Constants

        const float firstMarker = 0.15f;
        const float lastMarker = 1.0f - firstMarker;

        #endregion

        #region Public Properties

        /// <summary>
        /// The list of audio clips to crossfade between.
        /// </summary>
        public List<AudioClip> clips;

        /// <summary>
        /// The first AudioSource used for playback.
        /// </summary>
        public AudioSource firstSource;

        /// <summary>
        /// The second AudioSource used for playback.
        /// </summary>
        public AudioSource secondSource;

        #endregion

        #region Private Properties

        private AudioClip _current;
        private AudioClip _next;

        private AudioSource _currentSource;
        private AudioSource _nextSource;

        private bool _currentHasReachedDeclining = false;
        private bool _choosenNextClip = false;

        #endregion

        #region Unity Methods

        private void Start()
        {
            firstSource.loop = false;
            secondSource.loop = false;

            _currentSource = firstSource;
            _nextSource = secondSource;

            _currentSource.clip = GetNextClip(null);
            _currentSource.Play();

            _nextSource.Stop();
            _nextSource.clip = null;
        }

        public void Update()
        {
            // Adjust volume of the current and next sources based on progress in the audio clips.
            if (_currentSource.isPlaying) { _currentSource.volume = VolumeFromProgress(_currentSource); }

            if (_nextSource.isPlaying) { _nextSource.volume = VolumeFromProgress(_nextSource); }

            // If the next source is not playing and the next clip hasn't been chosen, pick a new one.
            if (!_nextSource.isPlaying && !_choosenNextClip)
            {
                _nextSource.clip = GetNextClip(_currentSource.clip);
                _choosenNextClip = true;
            }

            // Swap sources when the current source has finished playing.
            if (!_currentSource.isPlaying) { SwapSources(); }

            // Start playing the next source when the current source has reached the lastMarker.
            if (GetProgress(_currentSource) > lastMarker && !_currentHasReachedDeclining)
            {
                _currentHasReachedDeclining = true;
                _nextSource.Play();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Start playing the next source.
        /// </summary>
        private void PlayNext()
        {
            _nextSource.Play();
        }

        /// <summary>
        /// Swap the current and next audio sources.
        /// </summary>
        private void SwapSources()
        {
            AudioSource temp = _currentSource;
            _currentSource = _nextSource;
            _nextSource = temp;

            _choosenNextClip = false;
            _currentHasReachedDeclining = false;
        }

        /// <summary>
        /// Calculate the volume based on the progress in the audio clip.
        /// </summary>
        private float VolumeFromProgress(AudioSource source)
        {
            float progress = GetProgress(source);

            if (progress < firstMarker) { return progress / firstMarker; }
            else if (progress > lastMarker) { return (1.0f - progress) / firstMarker; }
            else { return 1.0f; }
        }

        /// <summary>
        /// Calculate the progress of the current audio clip.
        /// </summary>
        private float GetProgress(AudioSource source)
        {
            if (source != null && source.clip != null) { return (float)source.timeSamples / (float)source.clip.samples; }
            else { return 1.0f; }
        }

        /// <summary>
        /// Get the next audio clip, excluding the current clip if available.
        /// </summary>
        private AudioClip GetNextClip(AudioClip current)
        {
            if (clips.Count <= 1) { return null; }

            AudioClip clip = clips.PickRandom();
            while (clip == current && clip != null) { clip = clips.PickRandom(); }

            return clip;
        }

        #endregion
    }
}
