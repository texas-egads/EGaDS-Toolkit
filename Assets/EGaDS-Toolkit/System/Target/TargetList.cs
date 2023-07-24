using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.GraphicsBuffer;

namespace egads.system.target
{
	public class TargetList<T> where T : MonoBehaviour, ITarget
	{
        #region Events

        public event System.Action<T> focusChanged;

        #endregion

        #region Public Properties

        public bool hasObjects => _targets.Count > 0;
        public int Count => _targets.Count;
        public T current
		{
			get
			{
				if (_targets.Count > 0) { return _targets[_targets.Count - 1]; }
				else { return default(T); }
			}
		}

        #endregion

        #region Private Properties

        protected List<T> _targets = new List<T>();

        #endregion

        #region Public Methods

		public void Add(T target)
		{
			// Check if valid
			if (target == null) { return; }
				
			if (_targets.Contains(target)) { return; }

			_targets.Add(target);

			OnFocusChanged();
		}

		public void Remove(T target)
		{
			// Check if valid
			if (target == null || !_targets.Contains(target)) { return; }	

			if (target == current)
			{
				_targets.Remove(target);
				
				OnFocusChanged();
			}
			else { _targets.Remove(target); }
		}

		public void Clear()
		{
			if (_targets.Count > 0)
			{
				_targets.Clear();
				OnFocusChanged();
			}
		}

		public bool Contains(T focusObject) => _targets.Contains(focusObject);

		public void SortByNearest(Vector2 pos)
		{
			if (_targets.Count <= 1) { return; }

			T oldFocus = current;

			_targets.Sort((firstObject, secondObject) => Vector2.SqrMagnitude(secondObject.position2D - pos).CompareTo(Vector2.SqrMagnitude(firstObject.position2D - pos)));

			if (oldFocus != current) { OnFocusChanged(); }
		}

        #endregion

        #region Private Methods

        private void OnFocusChanged()
		{
			if (focusChanged != null) { focusChanged(current); }
		}

        #endregion
    }

}
