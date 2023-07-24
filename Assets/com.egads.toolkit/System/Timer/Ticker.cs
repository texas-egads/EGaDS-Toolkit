using UnityEngine;

namespace egads.system.timer
{
	/// <summary>
	/// Timer helper which 'ticks' at every duration
	/// Gets updated from outside
	/// </summary>
	public class Ticker
	{
        #region Delegates

        public delegate void TickAction();

        #endregion

        #region Private Properties

        private TickAction _tickEvent;
		private float _interval = 1.0f;
		private float _tickTime = 0;

        #endregion

        #region Constructor

        public Ticker(TickAction tickHandler, float interval = 1.0f)
		{
			_tickEvent = tickHandler;
			_interval = interval;
		}

        #endregion

        #region Public Methods

        public void Update()
		{
			_tickTime += Time.deltaTime;

			if (_tickTime >= _interval)
			{
				if (_tickEvent != null) { _tickEvent(); }
				_tickTime = 0;
			}
		}

		public void Reset()
		{
			_tickTime = 0;
		}

        #endregion
    }
}
