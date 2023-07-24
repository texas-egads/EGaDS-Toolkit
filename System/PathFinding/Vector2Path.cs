using System.Collections.Generic;
using UnityEngine;

namespace egads.system.pathFinding
{
	public class Vector2Path
	{
        #region Private Properties

        private List<Vector2> _path = new List<Vector2>();
		private int _count = 0;
		private int _index = 0;

		private bool _hasFinished = false;

        #endregion

        #region Public Properties
        public bool hasFinished => _hasFinished;
		public bool isValid => _count > 0;
		public int Count => _count;
		public Vector2 this[int index] => _path[index];
		public Vector2 CurrentPosition => _path[_index];

        #endregion

        #region Constructor

        public Vector2Path(int allocationSize = 50)
		{
			for (int i = 0; i < allocationSize; i++) { _path.Add(new Vector2(0, 0)); }
		}

        #endregion

        #region Public Methods

        public void AddPosition(float x, float y)
		{
			if (_count == _path.Count)
			{
				_path.Add(new Vector2(x, y));
				_count++;
				_hasFinished = false;
				return;
			}

			_path[_count] = new Vector2(x, y);
			_count++;
			_hasFinished = false;
		}

		public void NextPosition()
		{
			_index++;

			if (_index >= _count) { _hasFinished = true; }
		}

		public void Clear()
		{
			_index = 0;
			_count = 0;
			_hasFinished = true;
		}

		public override string ToString()
		{
			string posString = "";

			for (int i = 0; i < _count; i++) { posString += "[" + _path[i] + "] "; }

			return posString;
		}

        #endregion
    }
}