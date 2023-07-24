using UnityEngine;

namespace egads.system.timer
{
	/// <summary>
	/// Simpler Timer, it has to be updated from outside
	/// </summary>
	public class Timer
	{

        #region Public Properties

        public bool hasEnded => _elapsedTime >= _duration;
		public float progress
		{
			get
			{
				float p = _elapsedTime / _duration;

				if (p < 0) { p = 0f; }
				if (p > 1f) { p = 1f; }

				return p;
			}
		}
		public float invertedProgress => 1f - progress;
		public float time => _elapsedTime;

        #endregion

        #region Private Properties

        private float _duration = 3.0f;
		public float duration { get { return _duration; } }

		private float _elapsedTime = 0;

        #endregion

        #region Constructor

        public Timer(float duration)
		{
			_duration = duration;
		}

        #endregion

        #region Public Methods

        public void Update()
		{
			_elapsedTime += Time.deltaTime;
		}

		public void Reset()
		{
			_elapsedTime = 0;
		}

		public void Stop()
		{
			_elapsedTime = _duration;
		}

		public void SetDuration(float duration)
		{
			_duration = duration;
		}

		public void SetRandomStart(float progress = 1.0f)
		{
			_elapsedTime = Random.Range(0, _duration * progress);
		}

		/// <summary>
		/// Builds a string of the remaining time i.e. "1:59"
		/// </summary>
		/// <returns></returns>
		public string ToCountdownString()
		{
			int remaining = Mathf.CeilToInt(_duration - _elapsedTime);

			if (remaining < 0) { return "0:00"; }

			int minutes = remaining / 60;
			int seconds = remaining % 60;

			string timeString = minutes.ToString() + ":";
			if (seconds < 10) { timeString += "0"; }
			timeString += seconds.ToString();

			return timeString;
		}

        #endregion
    }
}