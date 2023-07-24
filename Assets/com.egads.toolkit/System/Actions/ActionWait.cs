using egads.system.timer;

namespace egads.system.actions
{
	public class ActionWait : IActionQueueElement
	{
        #region Private Properties

        private Timer _timer;

		private float _duration;

        #endregion

        #region Public Methods
        public ActionWait(float duration)
		{
			_duration = duration;
		}

		public void OnStart()
		{
			_timer = new Timer(_duration);
		}

		public void Update()
		{
			_timer.Update();
		}

		public void OnExit()
		{

		}

		public bool hasEnded
		{
			get
			{
				return _timer.hasEnded;
			}
		}

        #endregion
    }
}