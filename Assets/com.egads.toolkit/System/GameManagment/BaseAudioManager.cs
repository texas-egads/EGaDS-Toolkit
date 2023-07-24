using UnityEngine;
using System.Collections.Generic;
using egads.system.timer;

namespace egads.system.gameManagement
{
	[RequireComponent(typeof(AudioSource))]
	public class BaseAudioManager : MonoBehaviour
	{
        #region Constants

        const float PROGRESS_UNTIL_NEXT_PLAY = 0.7f;

        #endregion

        #region Private Properties

        [SerializeField]
		private AudioClip _buttonSound;
		private AudioSource _source;
		private Dictionary<string, Timer> _playedList = new Dictionary<string, Timer>();

        #endregion

        #region Unity Methods

        private void Awake()
		{
			_source = GetComponent<AudioSource>();
		}

		private void Update()
		{
			CleanPlayedList();
		}

        #endregion

        #region Public Methods
        public void PlayButtonSound()
		{
			Play(_buttonSound);
		}

		// Plays a random command sound
		public void PlayRandomSound(AudioClip[] clips)
		{
			AudioClip commandSound = clips[Random.Range(0, clips.Length)];
			Play(commandSound);
		}

		/// <summary>
		/// Plays an Audioclip if it was not played in PROGRESS_UNTIL_NEXT_PLAY of duration
		/// </summary>
		/// <param name="clip"></param>
		public void PlayOnce(AudioClip clip, float volume = 1.0f)
		{
			if (clip == null) { return; }

			string type = clip.name;

			if (!_playedList.ContainsKey(type)) { _playedList[type] = new Timer(clip.length * PROGRESS_UNTIL_NEXT_PLAY); }
			else
			{
				if (_playedList[type].hasEnded) { _playedList[type].Reset(); }
				else { return; }
			}

			PlayWithVariation(clip, volume);
		}

		public void Play(AudioClip clip, float volume = 1.0f)
		{
			if (clip != null) { _source.PlayOneShot(clip, _source.volume * volume); }
		}

		public void PlayWithVariation(AudioClip clip, float volume = 1.0f)
		{
			_source.pitch = Random.Range(.97f, 1.0f);
			float volumeVariation = Random.Range(.85f, 1f);
			_source.PlayOneShot(clip, _source.volume * volume * volumeVariation);
		}

        #endregion

        #region Private Methods

        // Removes elapsed entries in the playedList
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