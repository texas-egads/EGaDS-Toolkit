using UnityEngine;

namespace egads.system.actors
{
	public class Energy
	{
        #region Constants

        const float min = 0;

        #endregion

        #region Public Properties

        public float current
        {
            get { return _current; }
            private set { _current = Mathf.Clamp(value, min, _max); }
        }
		public float max => _max;
		public float missingAmount => (_max - _current);
		public float proportion => (_current / _max);
		public bool isFull => (_current >= _max);
		public bool isEmpty => (_current <= min);
		public float range => (_max - min);

        #endregion

        #region Private Properties

        private float _current = 0;
		private float _max = 1;
		private float _startValue;

        #endregion

        #region Constructor

        public Energy(float max)
		{
			_max = max;
			_current = max;
			_startValue = _current;
		}

		public Energy(float current, float max)
		{
			_max = max;
			this.current = current;
			_startValue = current;
		}

        #endregion

        #region Add Methods

        public void AddPortion(float portion)
		{
			Add(portion * range);
		}

		public void AddFull()
		{
			Add(_max - _current);
		}

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

        public void LosePortion(float portion)
		{
			Lose(portion * range);
		}

		public void LoseAll()
		{
			Lose(_current);
		}

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

        public void Reset()
		{
			_current = _startValue;
		}

		// For balancing
		public void Scale(float factor)
		{
			_max *= factor;
			_current *= factor;
			_startValue *= factor;
		}

		public float GetAmountThatWillBeAdded(float maxPossibleAmount) => Mathf.Min(maxPossibleAmount, missingAmount);

		public override string ToString() => ((int)_current).ToString() + " / " + ((int)max).ToString();

        #endregion

        #region Event System

        #region Delegates

        public delegate void EnergyDelegate();
		public delegate void EnergyChangedDelegate(float amount);

        #endregion

        #region Events

        public event EnergyDelegate valueChanged;
		public event EnergyDelegate gotFull;
		public event EnergyChangedDelegate gotEnergy;
		public event EnergyChangedDelegate lostEnergy;

        #endregion

        #region Event Methods

        private void OnValueChanged()
		{
			if (valueChanged != null) { valueChanged(); }
		}

		private void OnGotFull()
		{
			if (gotFull != null) { gotFull(); }
		}

		private void OnGotEnergy(float amount)
		{
			if (gotEnergy != null && amount >= 0) { gotEnergy(amount); }
		}

		private void OnLostEnergy(float amount)
		{
			if (lostEnergy != null && amount > 0) { lostEnergy(amount); }
		}

        #endregion

        #endregion
    }
}