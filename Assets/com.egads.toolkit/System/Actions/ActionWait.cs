using egads.system.timer;

namespace egads.system.actions
{
    /// <summary>
    /// Represents an action that adds a time delay (wait) before completing.
    /// </summary>
    public class ActionWait : IActionQueueElement
    {
        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether the wait action has ended.
        /// </summary>
        public bool hasEnded => _timer.hasEnded;

        #endregion

        #region Private Properties

        private Timer _timer;
        private float _duration;

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new instance of the ActionWait class with the specified duration.
        /// </summary>
        /// <param name="duration">The duration of the wait action in seconds.</param>
        public ActionWait(float duration)
        {
            _duration = duration;
        }

        /// <summary>
        /// Called when the wait action starts. Initializes the timer with the specified duration.
        /// </summary>
        public void OnStart()
        {
            _timer = new Timer(_duration);
        }

        /// <summary>
        /// Updates the wait action by updating the timer.
        /// </summary>
        public void Update()
        {
            _timer.Update();
        }

        /// <summary>
        /// Called when the wait action is completed or interrupted. It does nothing in this case.
        /// </summary>
        public void OnExit()
        {
            // Empty: The wait action does not require any cleanup when it completes.
        }

        #endregion
    }
}
