using UnityEngine;
using egads.system.timer;

namespace egads.system.audio
{
	public class RandomSound : MonoBehaviour
	{
        #region Public Properties

        public AudioSource source;

		public float pauseIntervalMin;
		public float pauseIntervalMax;

        #endregion

        #region Private Properties

        private Timer _timer = null;

        #endregion

        #region Unity Methods

        private void Awake()
		{
			if (source == null) { source = GetComponent<AudioSource>(); }

			if (source != null) { source.loop = false; }

			StartNewTimer();
		}

		private void Update()
		{
			if (source != null && !source.isPlaying)
			{
				if (_timer == null) { StartNewTimer(); }
				else { _timer.Update(); }

				if (_timer.hasEnded)
				{
					_timer = null;
					source.Play();
				}
			}
		}

        #endregion

        #region Private Methods

        private void StartNewTimer()
		{
			_timer = new Timer(Random.Range(pauseIntervalMin, pauseIntervalMax));
		}

        #endregion
    }
}