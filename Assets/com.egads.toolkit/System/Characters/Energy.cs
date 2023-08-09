using UnityEngine;

namespace egads.system.characters
{
    /// <summary>
    /// Represents an energy system with current and maximum energy values and various utility methods.
    /// </summary>
    public class Energy
    {
        #region Constants

        // The minimum value for the energy.
        private const float min = 0;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the current energy value, clamped between 0 and the maximum value.
        /// </summary>
        public float current
        {
            get { return _current; }
            private set { _current = Mathf.Clamp(value, min, _max); }
        }

        /// <summary>
        /// Gets the maximum energy value.
        /// </summary>
        public float max => _max;

        /// <summary>
        /// Gets the amount of energy missing to reach the maximum value.
        /// </summary>
        public float missingAmount => (_max - _current);

        /// <summary>
        /// Gets the proportion of current energy to the maximum value (0 to 1).
        /// </summary>
        public float proportion => (_current / _max);

        /// <summary>
        /// Gets a value indicating whether the energy is at its maximum capacity.
        /// </summary>
        public bool isFull => (_current >= _max);

        /// <summary>
        /// Gets a value indicating whether the energy is empty (reached the minimum value).
        /// </summary>
        public bool isEmpty => (_current <= min);

        /// <summary>
        /// Gets the range of energy values (difference between the maximum and minimum).
        /// </summary>
        public float range => (_max - min);

        #endregion

        #region Private Properties

        private float _current = 0;
        private float _max = 1;
        private float _startValue;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor for the Energy class with specified maximum energy value.
        /// </summary>
        /// <param name="max">The maximum energy value.</param>
        public Energy(float max)
        {
            _max = max;
            _current = max;
            _startValue = _current;
        }

        /// <summary>
        /// Constructor for the Energy class with specified current and maximum energy values.
        /// </summary>
        /// <param name="current">The current energy value.</param>
        /// <param name="max">The maximum energy value.</param>
        public Energy(float current, float max)
        {
            _max = max;
            this.current = current;
            _startValue = current;
        }

        #endregion

        #region Add Methods

        /// <summary>
        /// Adds a portion of the maximum possible energy to the current energy value.
        /// </summary>
        /// <param name="portion">The portion of maximum possible energy to add (0 to 1).</param>
        public void AddPortion(float portion)
        {
            Add(portion * range);
        }

        /// <summary>
        /// Adds the maximum possible energy to the current energy value.
        /// </summary>
        public void AddFull()
        {
            Add(_max - _current);
        }

        /// <summary>
        /// Adds a specified amount to the current energy value.
        /// </summary>
        /// <param name="amount">The amount of energy to add.</param>
        public void Add(float amount)
        {
            float before = _current;

            current += amount;

            // Check for events now
            OnValueChanged();
            OnGotEnergy(_current - before);
            if (isFull && before < _current) { OnGotFull(); }
        }

        #endregion

        #region Remove Methods

        /// <summary>
        /// Loses a portion of the maximum possible energy from the current energy value.
        /// </summary>
        /// <param name="portion">The portion of maximum possible energy to lose (0 to 1).</param>
        public void LosePortion(float portion)
        {
            Lose(portion * range);
        }

        /// <summary>
        /// Loses all the current energy.
        /// </summary>
        public void LoseAll()
        {
            Lose(_current);
        }

        /// <summary>
        /// Loses a specified amount from the current energy value.
        /// </summary>
        /// <param name="amount">The amount of energy to lose.</param>
        public void Lose(float amount)
        {
            if (isEmpty) { return; }

            float before = _current;

            current -= amount;

            // Check for events now
            OnLostEnergy(before - _current);
            OnValueChanged();
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Resets the current energy value to its starting value.
        /// </summary>
        public void Reset()
        {
            _current = _startValue;
        }

        /// <summary>
        /// Scales the maximum and current energy values by a given factor for balancing.
        /// </summary>
        /// <param name="factor">The scaling factor.</param>
        public void Scale(float factor)
        {
            _max *= factor;
            _current *= factor;
            _startValue *= factor;
        }

        /// <summary>
        /// Gets the maximum possible amount that can be added to the current energy value.
        /// </summary>
        /// <param name="maxPossibleAmount">The maximum possible amount to add.</param>
        /// <returns>The actual amount that will be added.</returns>
        public float GetAmountThatWillBeAdded(float maxPossibleAmount) => Mathf.Min(maxPossibleAmount, missingAmount);

        /// <summary>
        /// Returns the current energy value and the maximum energy value as a formatted string.
        /// </summary>
        /// <returns>The energy values as a string (e.g., "25 / 50").</returns>
        public override string ToString() => ((int)_current).ToString() + " / " + ((int)max).ToString();

        #endregion

        #region Event System

        #region Delegates

        /// <summary>
        /// Delegate for events when the energy value changes.
        /// </summary>
        public delegate void EnergyDelegate();

        /// <summary>
        /// Delegate for events when the energy value changes with a parameter for the amount changed.
        /// </summary>
        /// <param name="amount">The amount by which the energy value changed.</param>
        public delegate void EnergyChangedDelegate(float amount);

        #endregion

        #region Events

        /// <summary>
        /// Event triggered when the energy value changes.
        /// </summary>
        public event EnergyDelegate valueChanged;

        /// <summary>
        /// Event triggered when the energy value reaches its maximum capacity.
        /// </summary>
        public event EnergyDelegate gotFull;

        /// <summary>
        /// Event triggered when energy is gained.
        /// </summary>
        public event EnergyChangedDelegate gotEnergy;

        /// <summary>
        /// Event triggered when energy is lost.
        /// </summary>
        public event EnergyChangedDelegate lostEnergy;

        #endregion

        #region Event Methods

        /// <summary>
        /// Triggers the valueChanged event if there are subscribers.
        /// </summary>
        private void OnValueChanged()
        {
            if (valueChanged != null) { valueChanged(); }
        }

        /// <summary>
        /// Triggers the gotFull event if there are subscribers.
        /// </summary>
        private void OnGotFull()
        {
            if (gotFull != null) { gotFull(); }
        }

        /// <summary>
        /// Triggers the gotEnergy event with the specified amount if there are subscribers.
        /// </summary>
        /// <param name="amount">The amount of energy gained.</param>
        private void OnGotEnergy(float amount)
        {
            if (gotEnergy != null && amount >= 0) { gotEnergy(amount); }
        }

        /// <summary>
        /// Triggers the lostEnergy event with the specified amount if there are subscribers.
        /// </summary>
        /// <param name="amount">The amount of energy lost.</param>
        private void OnLostEnergy(float amount)
        {
            if (lostEnergy != null && amount > 0) { lostEnergy(amount); }
        }

        #endregion

        #endregion
    }
}