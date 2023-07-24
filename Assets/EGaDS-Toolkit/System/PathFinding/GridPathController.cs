using System.Collections.Generic;

namespace egads.system.pathFinding
{
	/// <summary>
	/// A* pathfinding in square based fields
	/// </summary>
	public class GridPathController
	{
        #region Delegates

        public delegate bool MovementPossibleCheck(int fromColumn, int fromRow, int column, int row);

        #endregion

        #region Public Properties

        public GridPathReversed gridPath = new GridPathReversed();

        #endregion

        #region Private Properties

        private int _rowCount = 1;
		private int _columnCount = 1;
		private int _fieldCount = 1;

		private GridPathNode _nearestPoint;
		private int _nearestDistance = 0;
		private int _lastTimeNearestPointPushedForward;

        // Check values for different directions; standard is four directions
        private int[] _columnCheck = { 0, -1, 1, 0 };
		private int[] _rowCheck = { -1, 0, 0, 1 };
		private int[] _costOfMovement = { 10, 10, 10, 10 };
		private int _checkLength = 4;
		private int _heuristicValue = 1;

        #endregion

        #region Grid Path Node Properties

        private GridPathNode[] _grid;
		public GridPathNode[] grid => _grid;
        private List<GridPathNode> _openList = new List<GridPathNode>();

        #endregion

        #region Constructor

        public GridPathController()
		{
			// Not needed
		}

        #endregion

        #region Public Methods

        public void SetDimensions(int columnCount, int rowCount)
		{
			_columnCount = columnCount;
			_rowCount = rowCount;

			// Prepare closed list
			_grid = new GridPathNode[rowCount * columnCount];
			for (int row = 0; row < _rowCount; row++)
			{
				for (int column = 0; column < _columnCount; column++)
				{
					int index = row * _columnCount + column;
					_grid[index] = new GridPathNode(column, row, index);
				}
			}

			_fieldCount = _rowCount * _columnCount;

			gridPath.SetDimensions(_columnCount, _rowCount);
		}

		public void AllowDiagonalMovement(bool allowed)
		{
			if (allowed)
			{
				_columnCheck = new int[] { -1, 1, 1, -1, 0, -1, 1, 0 };
				_rowCheck = new int[] { 1, 1, -1, -1, -1, 0, 0, 1 };
				_costOfMovement = new int[] { 16, 16, 16, 16, 10, 10, 10, 10 };
				_checkLength = 8;
			}
			else
			{
				_columnCheck = new int[] { 0, -1, 1, 0 };
				_rowCheck = new int[] { -1, 0, 0, 1 };
				_costOfMovement = new int[] { 10, 10, 10, 10 };
				_checkLength = 4;
			}
		}

		/// <summary>
		/// Will hardcode possible directions of all nodes into the nodes itself
		/// </summary>
		/// <param name="movementPossible"></param>
		public void CalculatePossibleDirections(MovementPossibleCheck movementPossible = null)
		{
			for (int row = 0; row < _rowCount; row++)
			{
				for (int column = 0; column < _columnCount; column++)
				{
					GridPathNode currentNode = _grid[row * _columnCount + column];

					for (int i = 0; i < _checkLength; i++)
					{
						int checkRow = currentNode.row + _rowCheck[i];
						int checkColumn = currentNode.column + _columnCheck[i];
						int checkField = checkRow * _columnCount + checkColumn;

						// If new position out of bounds, ignore it
						if (checkRow < 0 || checkColumn < 0 || checkRow >= _rowCount || checkColumn >= _columnCount) { continue; }

						GridPathNode checkNode = _grid[checkField];

						// Movement not possible => ignore
						if (movementPossible == null)
						{
							if (checkNode.walkable == false) { continue; }
						}
						else
						{
							if (movementPossible(currentNode.column, currentNode.row, checkColumn, checkRow) == false) { continue; }
						}

						currentNode.AddDirection(checkNode, _costOfMovement[i]);
					}
				}
			}
		}

		// Calculates path from startPos to endPos
		public bool FindPath(GridPosition startPos, GridPosition endPos, MovementPossibleCheck movementPossible = null, bool saveNearestPath = true, int maxNodeChecksWithoutProgress = 0)
		{
			// Assume no path found
			gridPath.Clear();

			if (startPos.row < 0 || startPos.column < 0 || endPos.row < 0 || endPos.column < 0 || startPos.column >= _columnCount || startPos.row >= _rowCount || endPos.column >= _columnCount || endPos.row >= _rowCount) { return false; }

			// Reset open and closed list
			_openList.Clear();
			for (int field = 0; field < _fieldCount; field++)
			{
				GridPathNode node = _grid[field];
				if (node.closed) { node.closed = false; }
			}

			// Set target values
			int targetNodeIndex = endPos.row * _columnCount + endPos.column;
			GridPathNode targetNode = _grid[targetNodeIndex];

			// Check if end point is walkable and then decide if using brute force or sophisticated
			// Brute force works better when no path will be found and maxNodeChecksWithoutProgress is not set
			bool bruteForce = false;
			bool movementFromStartToEndIsPossible = false;

			if (movementPossible != null) { movementFromStartToEndIsPossible = movementPossible(startPos.column, startPos.row, endPos.column, endPos.row); }
			else { movementFromStartToEndIsPossible = targetNode.walkable; }
				

			// Check if sophisticated or brute force approach
			if (!movementFromStartToEndIsPossible && maxNodeChecksWithoutProgress <= 0)
			{
				// Brute force
				_heuristicValue = 1;
				bruteForce = true;
			}
			else { _heuristicValue = 10; }

			// Set nearest point
			_nearestDistance = int.MaxValue;
			_lastTimeNearestPointPushedForward = 0;

			// Add the starting point to the open list
			int startNodeIndex = startPos.row * _columnCount + startPos.column;
			GridPathNode startNode = _grid[startNodeIndex];
			_openList.Add(startNode);
			startNode.h = GetHeuristic(startPos.row, startPos.column, endPos.row, endPos.column);
            // Starting point is also nearest point at the moment
            startNode.opened = true;
			_nearestPoint = startNode;   

            // While not found and still nodes to check and not the maximum of checks without real progress
            while (targetNode.closed == false && _openList.Count > 0 && (maxNodeChecksWithoutProgress <= 0 || _lastTimeNearestPointPushedForward < maxNodeChecksWithoutProgress))
			{
				// First node in the openList will become the current node
				GridPathNode currentNode = _openList[0];

				// Save nearest field
				if (currentNode.h < _nearestDistance)
				{
					_nearestDistance = currentNode.h;
					_nearestPoint = currentNode;
					_lastTimeNearestPointPushedForward = 0;
				}
				else
				{
					_lastTimeNearestPointPushedForward++;
				}

				// Mark current node as closed
				currentNode.closed = true;
				currentNode.opened = false;
				_openList.RemoveAt(0);

				for (int i = 0; i < currentNode.directionCount; i++)
				{
					GridPathNode.GridMovement gridMovement = currentNode.directions[i];
					GridPathNode checkNode = gridMovement.node;

					// If node is already closed, ignore it
					if (checkNode.closed)
						continue;

					// Movement not possible => ignore
					if (movementPossible == null)
					{
						if (checkNode.walkable == false)
							continue;
					}
					else
					{
						if (movementPossible(currentNode.column, currentNode.row, checkNode.column, checkNode.row) == false)
							continue;
					}

					// Possible movement cost including path to currentNode
					int g = currentNode.g + gridMovement.movementCost;

					if (checkNode.opened == false)
					{
						checkNode.parentIndex = currentNode.index;

						checkNode.h = GetHeuristic(checkNode.row, checkNode.column, endPos.row, endPos.column);
						checkNode.g = g;
						checkNode.f = checkNode.g + checkNode.h;

						checkNode.opened = true;

						if (bruteForce)
							_openList.Add(checkNode);
						else
							AddToOpenList(checkNode);
					}
					else
					{
						// CheckNode is on open list
						// If path to this checkNode is better from the current Node, update 
						if (checkNode.g > g)
						{
							// Update parent to the current Node
							checkNode.parentIndex = currentNode.index;

							// Update path cost, checkNode.h already calculated
							checkNode.g = g;
							checkNode.f = checkNode.g + checkNode.h;

							// Do not bother to move node to correct spot in open list,
							// This way it's faster
						}
					}
				}
			}

			// Clear opened values for next search
			int openListCount = _openList.Count;
			for (int i = 0; i < openListCount; i++)
			{
				GridPathNode node = _openList[i];
				node.opened = false;
			}

			// if target found
			if (targetNode.closed)
			{
				CreatePath(startNodeIndex, targetNodeIndex);
				return true;
			}
			// Save nearest path if wished
			else if (saveNearestPath)
			{
				CreatePath(startNodeIndex, _nearestPoint.row * _columnCount + _nearestPoint.column);
				return false;
			}
			// No path and no nearest point
			else { return false; }
		}

        #endregion

        #region Private Methods

        private void CreatePath(int startField, int targetField)
		{
			while (startField != targetField && targetField > 0)
			{
				GridPathNode node = _grid[targetField];

				gridPath.AddPosition(node.column, node.row);

				// Get next position
				targetField = node.parentIndex;
			}
		}

		private void AddToOpenList(GridPathNode newNode)
		{
			int count = _openList.Count;
			for (int i = 0; i < count; i++)
			{
				if (newNode.f < _openList[i].f)
				{
					_openList.Insert(i, newNode);
					return;
				}
			}

			_openList.Add(newNode);
		}

		// Calculates heuristic by Manhattan distance
		private int GetHeuristic(int startRow, int startColumn, int targetRow, int targetColumn)
		{
			return (UnityEngine.Mathf.Abs(startRow - targetRow) + UnityEngine.Mathf.Abs(startColumn - targetColumn)) * _heuristicValue;
		}

        #endregion
    }
}