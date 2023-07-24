using UnityEngine;

namespace egads.system.pathFinding
{
	/// <summary>
	/// Position in a square-based map with rows and columns
	/// Rows go up, columns go right, origin at bottom-left
	/// </summary>
	[System.Serializable]
	public struct GridPosition
	{
        #region Public Properties

        public int row;
		public int column;

        #endregion

        #region Constructor

        public GridPosition(int column, int row)
		{
			this.row = row;
			this.column = column;
		}

        #endregion

        #region Public Methods

        // Calculate Manhattan Distance to other position
        public int GetDistance(GridPosition position) => Mathf.Abs(this.column - position.column) + UnityEngine.Mathf.Abs(this.row - position.row);

		// New position from offset row and offset column
		public GridPosition NewPositionFromOffset(int columnOffset, int rowOffset) => new GridPosition(column + columnOffset, row + rowOffset);

		// Calculate direction from this position to other position
		public GridDirection GetDirection(GridPosition position)
		{
			// Top
			if (position.row < this.row) { return GridDirection.North; }
			// Right
			if (position.column > this.column) { return GridDirection.East; }
			// Bottom
			if (position.row > this.row) { return GridDirection.South; }
			// Left
			return GridDirection.West;
		}

		public GridDirection GetBestDirection(GridPosition position)
		{
			GridPosition offset = position - this;
			if (Mathf.Abs(offset.column) > Mathf.Abs(offset.row))
			{
				if (offset.column > 0) { return GridDirection.East; }
				else { return GridDirection.West; }
			}
			else
			{
				if (offset.row > 0) { return GridDirection.North; }
				else { return GridDirection.South; }
			}
		}

		public GridPosition north => GetAdjacentPosition(GridDirection.North);
		public GridPosition east => GetAdjacentPosition(GridDirection.East);
		public GridPosition west => GetAdjacentPosition(GridDirection.West);
		public GridPosition south => GetAdjacentPosition(GridDirection.South);

		// Get position next to this one
		public GridPosition GetAdjacentPosition(GridDirection direction)
		{
			GridPosition position = new GridPosition(column, row);

			// Top
			if (direction == GridDirection.North) { position.row++; }
			// Right
			if (direction == GridDirection.East) { position.column++; }
			// Bottom
			if (direction == GridDirection.South) { position.row--; }
			// Left
			if (direction == GridDirection.West) { position.column--; }

			return position;
		}

		public static GridPosition operator +(GridPosition posOne, GridPosition posTwo) => posOne.NewPositionFromOffset(posTwo.column, posTwo.row);

		public static GridPosition operator -(GridPosition posOne, GridPosition posTwo) => posOne.NewPositionFromOffset(-posTwo.column, -posTwo.row);

		public static GridPosition operator +(GridPosition pos, GridDirection direction) => pos.GetAdjacentPosition(direction);

		public static bool operator ==(GridPosition pos, GridPosition otherPos)
		{
			if (pos.row == otherPos.row && pos.column == otherPos.column) { return true; }
			else { return false; }
		}

		public static bool operator !=(GridPosition pos, GridPosition otherPos)
		{
			if (pos.row != otherPos.row || pos.column != otherPos.column) { return true; }
			else { return false; }
		}

		public override int GetHashCode()
		{
			int hash = 13;
			hash = (hash * 7) + column.GetHashCode();
			hash = (hash * 7) + row.GetHashCode();
			return hash;
		}

		public override bool Equals(object obj)
		{
			if (obj is GridPosition) { return this == (GridPosition)obj; }
			return false;
		}

		public override string ToString() => "Position: x " + column + " y " + row;

		public bool IsAdjacentOrDiagonal(GridPosition checkPoint)
		{
			int distanceX = Mathf.Abs(this.column - checkPoint.column);
			int distanceY = Mathf.Abs(this.row - checkPoint.row);

			if (distanceX <= 1 && distanceY <= 1) { return true; }
			else { return false; }
				
		}

		public bool IsAdjacent(GridPosition checkPoint)
		{
			int distanceX = Mathf.Abs(this.column - checkPoint.column);
			int distanceY = Mathf.Abs(this.row - checkPoint.row);
			return distanceX + distanceY == 1;
		}

        #endregion
    }
}