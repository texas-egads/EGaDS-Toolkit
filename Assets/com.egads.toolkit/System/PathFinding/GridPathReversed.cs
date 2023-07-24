using System.Collections.Generic;

namespace egads.system.pathFinding
{
	/// <summary>
	/// Data class that represents a path through a grid defined by rows and columns
	/// Positions are saved internally in reversed order because they are added in that order without known length
	/// Used by GridPathController to save the last calculated path without memory allocation
	/// </summary>
	public class GridPathReversed
	{
        #region Public Properties

        public int count = 0;
        public GridPosition this[int index] => _path[count - 1 - index];

        #endregion

        #region Private Properties

        private List<GridPosition> _path = new List<GridPosition>();

        #endregion

        #region Public Methods
        public void SetDimensions(int maxColumn, int maxRow)
		{
			int maxSize = (maxColumn + maxRow) * 5;

			for (int i = 0; i < maxSize; i++)
			{
				_path.Add(new GridPosition(0, 0));
			}
		}

		public void AddPosition(int column, int row)
		{
			if (count == _path.Count - 1)
			{
				_path.Add(new GridPosition(column, row));
				count++;
				return;
			}

			// For struct
			_path[count] = new GridPosition(column, row);

			count++;
		}

		public void Clear()
		{
			count = 0;
		}

        #endregion
    }
}