using UnityEngine;
using egads.system.timer;

namespace egads.system.audio
{
	public class RandomSoundVolume : MonoBehaviour
	{
        #region Public Properties

        public AudioSource source;

		public float intervalMin = 5.0f;
		public float intervalMax = 10.0f;

		public bool fadeInFadeOut = true;

        #endregion

        #region Private Properties

        private bool _active = true;
		private FadingTimer _timer;

        #endregion

        #region Unity Methods

        private void Awake()
		{
			if (source == null) { source = GetComponent<AudioSource>(); }

			source.loop = true;
			if (!source.isPlaying) { source.Play(); }

			StartNewTimer();
		}

		private void Update()
		{
			if (_timer.hasEnded)
			{
				_active = !_active;
				source.mute = !_active;
				StartNewTimer();
			}
			if (_active)
			{
				if (fadeInFadeOut) { source.volume = _timer.progress; }
				else { source.volume = 1.0f; }
			}
			_timer.Update();
		}

        #endregion

        #region Private Methods

        private void StartNewTimer()
		{
			_timer = new FadingTimer(intervalMin * 0.2f, Random.Range(intervalMin, intervalMax), intervalMin * 0.2f);

		}
        #endregion
    }
}