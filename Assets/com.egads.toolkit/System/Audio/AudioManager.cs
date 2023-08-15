using UnityEngine;
using System.Collections.Generic;
using egads.system.timer;

namespace egads.system.audio
{
    /// <summary>
    /// This class manages audio playback in the game and provides various methods for playing audio clips.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        #region Constants

        /// <summary>
        /// The progress threshold after which the same sound can be played again.
        /// </summary>
        const float PROGRESS_UNTIL_NEXT_PLAY = 0.7f;

        #endregion

        #region Private Properties

        [SerializeField]
        private AudioClip _buttonSound; // Sound to play for button interactions
        private AudioSource _source;    // Reference to the audio source component
        private Dictionary<string, Timer> _playedList = new Dictionary<string, Timer>(); // Keeps track of played audio clips

        #endregion

        #region Unity Methods

        /// <summary>
        /// Called when the object is initialized.
        /// </summary>
        private void Awake()
        {
            _source = GetComponent<AudioSource>(); // Get the AudioSource component
        }

        /// <summary>
        /// Called every frame.
        /// </summary>
        private void Update()
        {
            CleanPlayedList(); // Remove elapsed entries from the playedList
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Play the button interaction sound.
        /// </summary>
        public void PlayButtonSound()
        {
            Play(_buttonSound);
        }

        /// <summary>
        /// Play a random sound from the given array of audio clips.
        /// </summary>
        /// <param name="clips">Array of audio clips to choose from.</param>
        public void PlayRandomSound(AudioClip[] clips)
        {
            AudioClip commandSound = clips[Random.Range(0, clips.Length)];
            Play(commandSound);
        }

        /// <summary>
        /// Plays an audio clip if it was not played recently within a specified duration.
        /// </summary>
        /// <param name="clip">The audio clip to play.</param>
        /// <param name="volume">Volume level of the audio clip.</param>
        public void PlayOnce(AudioClip clip, float volume = 1.0f)
        {
            if (clip == null) { return; }

            string type = clip.name;

            // Check if the audio clip was played recently
            if (!_playedList.ContainsKey(type)) { _playedList[type] = new Timer(clip.length * PROGRESS_UNTIL_NEXT_PLAY); }
            else
            {
                if (_playedList[type].hasEnded) { _playedList[type].Reset(); }
                else { return; }
            }

            PlayWithVariation(clip, volume);
        }

        /// <summary>
        /// Play an audio clip with the given volume.
        /// </summary>
        /// <param name="clip">The audio clip to play.</param>
        /// <param name="volume">Volume level of the audio clip.</param>
        public void Play(AudioClip clip, float volume = 1.0f)
        {
            if (clip != null) { _source.PlayOneShot(clip, _source.volume * volume); }
        }

        /// <summary>
        /// Play an audio clip with pitch and volume variations.
        /// </summary>
        /// <param name="clip">The audio clip to play.</param>
        /// <param name="volume">Volume level of the audio clip.</param>
        public void PlayWithVariation(AudioClip clip, float volume = 1.0f)
        {
            _source.pitch = Random.Range(.97f, 1.0f);
            float volumeVariation = Random.Range(.85f, 1f);
            _source.PlayOneShot(clip, _source.volume * volume * volumeVariation);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Remove elapsed entries in the playedList.
        /// </summary>
        private void CleanPlayedList()
        {
            List<string> removables = new List<string>();

            // Check entries and mark for removal
            foreach (var item in _playedList)
            {
                if (item.Value != null) { item.Value.Update(); }

                if (item.Value.hasEnded) { removables.Add(item.Key); }
            }

            // Remove items
            foreach (var item in removables)
            {
                _playedList.Remove(item);
            }
        }

        #endregion
    }
}
