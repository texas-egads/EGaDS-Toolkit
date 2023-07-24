
namespace egads.system.pathFinding
{
	/// <summary>
	/// Used by the A* path finding algorithm
	/// </summary>
	public class GridPathNode
	{
        #region Grid Movement

        public struct GridMovement
		{
			public GridPathNode node;
			public int movementCost;

			public GridMovement(GridPathNode node, int movementCost)
			{
				this.node = node;
				this.movementCost = movementCost;
			}
		}

        #endregion

        #region Public Properties

        public int row;
		public int column;
		public int index;

		public int f;
		public int g;
		public int h;

		public int parentIndex;

		public bool closed;
		public bool opened;
		public bool walkable;

		public GridMovement[] directions;
		public int directionCount;

        #endregion

        #region Constructor

        public GridPathNode(int column, int row, int index)
		{
			this.column = column;
			this.row = row;
			this.index = index;

			parentIndex = -1;

			f = 0;
			g = 0;
			h = 0;

			closed = false;
			opened = false;
			walkable = false;

			directions = new GridMovement[8];
			directionCount = 0;
		}

        #endregion

        #region Public Methods

        public void SetWalkable(bool isWalkable)
		{
			walkable = isWalkable;
		}

		public void AddDirection(GridPathNode node, int movementCost)
		{
			GridMovement gridMovement = new GridMovement(node, movementCost);
			directions[directionCount] = gridMovement;
			directionCount++;
		}

        #endregion
    }
}
