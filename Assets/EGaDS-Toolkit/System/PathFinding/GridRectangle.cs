using UnityEngine;

namespace egads.system.pathFinding
{
	public class GridRectangle
	{
        #region Private Properties

        private GridPosition min;
		private GridPosition max;

		private int width;
		private int height;

        #endregion

        #region Constructor

        public GridRectangle(GridPosition posOne, GridPosition posTwo)
		{
			int minCol = Mathf.Min(posOne.column, posTwo.column);
			int minRow = Mathf.Min(posOne.row, posTwo.row);

			int maxCol = Mathf.Max(posOne.column, posTwo.column);
			int maxRow = Mathf.Max(posOne.row, posTwo.row);

			min = new GridPosition(minCol, minRow);
			max = new GridPosition(maxCol, maxRow);
		}

        #endregion

        #region Public Methods

        public bool Contains(GridPosition pos)
		{
			return pos.column >= min.column
				&& pos.column <= max.column
				&& pos.row >= min.row
				&& pos.row <= max.row;
		}

        #endregion
    }
}