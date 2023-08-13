using egads.system.timer;

namespace egads.system.orders
{
    /// <summary>
    /// Represents an order that adds a time delay (wait) before completing.
    /// </summary>
    public class OrderWait : IOrderQueueElement
    {
        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether the wait order has ended.
        /// </summary>
        public bool hasEnded => _timer.hasEnded;

        #endregion

        #region Private Properties

        private Timer _timer;
        private float _duration;

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new instance of the OrderWait class with the specified duration.
        /// </summary>
        /// <param name="duration">The duration of the wait order in seconds.</param>
        public OrderWait(float duration)
        {
            _duration = duration;
        }

        /// <summary>
        /// Called when the wait order starts. Initializes the timer with the specified duration.
        /// </summary>
        public void OnStart()
        {
            _timer = new Timer(_duration);
        }

        /// <summary>
        /// Updates the wait order by updating the timer.
        /// </summary>
        public void Update()
        {
            _timer.Update();
        }

        /// <summary>
        /// Called when the wait order is completed or interrupted. It does nothing in this case.
        /// </summary>
        public void OnExit()
        {
            // Empty: The wait order does not require any cleanup when it completes.
        }

        #endregion
    }
}
