using UnityEngine;

namespace egads.system.timer
{
	/// <summary>
	/// Simple Timer Class, which has to be updated from outside,
	/// Has even and odd intervals
	/// </summary>
	public class BlinkTimer
	{
        #region Public Properties

        public float duration = 3.0f;
		public float timeInterval = 0.4f;

        #endregion

        #region Private Properties

        private float _elapsedTime = 0;

        #endregion

        #region Constructor

        public BlinkTimer()
		{
			// Not needed
		}

        #endregion

        #region Public Methods

        public bool Update()
		{
			_elapsedTime += Time.deltaTime;

			return IsBlinkInterval();
		}

		/// <summary>
		/// check if this is a blinking interval at the moment
		/// </summary>
		/// <returns></returns>
		public bool IsBlinkInterval()
		{
			if ((int)(_elapsedTime / timeInterval) % 2 == 0) { return true; }
			else { return false; }
		}

		// check if time has elapsed
		public bool Finished() => _elapsedTime >= duration;

        #endregion
    }
}