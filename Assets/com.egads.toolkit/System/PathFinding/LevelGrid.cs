﻿using UnityEngine;
using UnityEngine.SceneManagement;
using egads.tools.utils;
using egads.system.gameManagement;

namespace egads.system.pathFinding
{
	public class LevelGrid : MonoBehaviour, IPathField
	{
        #region Debug Properties

        public bool drawDebug = false;

        protected Vector3 _debugDrawSize;
        protected Vector2 _lastTarget;

        protected Vector2Path _testPath;

        #endregion

        #region Field Size Properties

        // Cell size of the grid
        [SerializeField]
		private float _fieldSizeX = 0.35f;
		public float fieldSizeX => _fieldSizeX;
		private float _fieldSizeY = 0.35f;
		public float fieldSizeY => _fieldSizeY;
		protected bool _usesDifferentHeight = false;

		public int maxCheckNodesWithoutProgress = 0;

        #endregion

        #region Boundary Properties

        // Bounds
        protected float _startX;
		protected float _startY;
		protected int _columnCount;
		protected int _rowCount;

		protected GridPathController _pathController = new GridPathController();

		private bool _wasCreated = false;

        #endregion

        #region Layer Properties

        // Layers
        protected string[] _collisionLayers = new string[] {"Building", "AreaEffects", "Walls"};
        protected string[] collisionLayers => _collisionLayers;
		protected string[] _collisionLayersIncludingDynamic = new string[] { "Building", "AreaEffects", "Obstacles", "Walls" };
		protected string[] collisionLayersIncludingDynamic => _collisionLayersIncludingDynamic;

        #endregion

        #region Unity Methods

        protected virtual void Awake()
		{
			_debugDrawSize = new Vector3(_fieldSizeX, _fieldSizeY, _fieldSizeX);
		}

        protected virtual void Start()
        {
			if (!_wasCreated)
			{
				CheckFieldSizeY();
				CreateGrid(MainBase.Instance.levelBounds, _fieldSizeX, _fieldSizeY);
			}

			if (SceneManager.GetActiveScene().name == "pathfinding_test") { Test(); }
		}

        #endregion

        #region Public Methods

        public void Init(Rect rect)
		{
			CheckFieldSizeY();
			CreateGrid(rect, _fieldSizeX, _fieldSizeY);
		}

        public virtual void GetPath(Vector2 startPos, Vector2 endPos, Vector2Path vectorPath)
        {
            _lastTarget = endPos;

            GridPosition startGridPosition = GetGridPositionFromLevelPosition(startPos);
            GridPosition endGridPosition = GetGridPositionFromLevelPosition(endPos);

            _pathController.FindPath(startGridPosition, endGridPosition, MovementPossibleCheck, true, maxCheckNodesWithoutProgress);

            vectorPath.Clear();
            if (_pathController.gridPath.count > 0)
            {
                for (int i = 0; i < _pathController.gridPath.count; i++)
                {
                    Vector2 p = GetLevelPositionFromGridPosition(_pathController.gridPath[i]);

                    vectorPath.AddPosition(p.x, p.y);
                }
            }

            _testPath = vectorPath;
        }

        public void UpdateField(Bounds bounds)
        {
            int fromColumn = GetColumnFromX(bounds.min.x);
            int toColumn = GetColumnFromX(bounds.max.x);

            int fromRow = GetRowFromY(bounds.min.y);
            int toRow = GetRowFromY(bounds.max.y);

            int layerMask = Utilities.LayerMaskIncludingDefault(collisionLayersIncludingDynamic);

            CheckColliders(_pathController.grid, layerMask, fromColumn: fromColumn, fromRow: fromRow, toColumn: toColumn, toRow: toRow);            
        }

        #endregion

        #region Private Methods

        protected bool MovementPossibleCheck(int fromColumn, int fromRow, int column, int row) => _pathController.grid[row * _columnCount + column].walkable;

        private void CreateGrid(Rect boundaries, float fieldSizeX, float fieldSizeY)
        {
            // calculate grid bounds
            _startX = boundaries.x;
            _startY = boundaries.y;
            _columnCount = (int)Mathf.Ceil(boundaries.width / fieldSizeX);
            _rowCount = (int)Mathf.Ceil(boundaries.height / fieldSizeY);

            _pathController.AllowDiagonalMovement(true);
            _pathController.SetDimensions(rowCount: _rowCount, columnCount: _columnCount);
            
            // mark all possible collisions as true on the grid
            CheckColliders(_pathController.grid, Utilities.LayerMaskIncludingDefault(collisionLayers));
            // hardcode directions
            _pathController.CalculatePossibleDirections();

            // check again and include dynamic objects
            CheckColliders(_pathController.grid, Utilities.LayerMaskIncludingDefault(collisionLayersIncludingDynamic));

			_wasCreated = true;
        }

        private void CheckColliders(GridPathNode[] grid, int layerMask)
        {
            CheckColliders(grid, layerMask, 0, 0, _columnCount - 1, _rowCount - 1);
        }

        private void CheckColliders(GridPathNode[] grid, int layerMask, int fromColumn, int fromRow, int toColumn, int toRow)
        {
            // save raycastsHitTriggers setting
            bool beforeSetting = Physics2D.queriesHitTriggers;
            Physics2D.queriesHitTriggers = false;

            for (int row = fromRow; row <= toRow; row++)
            {
                for (int column = fromColumn; column <= toColumn; column++)
                {
                    // calculate position
                    float posX = _startX + column * _fieldSizeX + _fieldSizeX * 0.5f;
                    float posY = _startY + row * _fieldSizeY + _fieldSizeY * 0.5f;

                    // check for colliders
                    Collider2D coll = Physics2D.OverlapCircle(new Vector2(posX, posY), _fieldSizeX * 0.45f, layerMask);
                    if (coll == null) { grid[row * _columnCount + column].SetWalkable(true); }
                    else { grid[row * _columnCount + column].SetWalkable(false); }
                }
            }

            // restore raycastsHitTriggers setting
            Physics2D.queriesHitTriggers = beforeSetting;
        }

        #endregion

        #region Testing Methods

        /// <summary>
        /// Debug Draw, show grid and last searched path
        /// </summary>
        protected void OnDrawGizmos()
        {
            if (!drawDebug || !Application.isPlaying) { return; }
                
            // Draw all grid fields
            for (int row = 0; row < _rowCount; row++)
            {
                for (int column = 0; column < _columnCount; column++)
                {
                    if (_pathController.grid[row * _columnCount + column].walkable == false)
                    {
                        Gizmos.color = new Color(1.0f, 0, 0, 0.2f);
                        DrawMarker(row, column);
                    }

                    if (_pathController.grid[row * _columnCount + column].closed)
                    {
                        Gizmos.color = new Color(1.0f, 1.0f, 0, 0.3f);
                        DrawMarker(row, column);
                    }
                }
            }

            Gizmos.color = new Color(0, 1.0f, 0, 0.2f);

            if (_testPath != null)
            {
                for (int i = 0; i < _testPath.Count; i++)
                {
                    Vector3 center = new Vector3(_testPath[i].x, _testPath[i].y, 0);
                    Gizmos.DrawCube(center, _debugDrawSize);
                }

                // Draw 'last target' marker
                Gizmos.color = new Color(0, 0, 1.0f, 0.3f);
                Vector3 lastCenter = new Vector3(_lastTarget.x, _lastTarget.y, 0);
                Gizmos.DrawCube(lastCenter, _debugDrawSize);
            }
        }

        private void Test()
        {
            _testPath = new Vector2Path((_columnCount + _rowCount) * 5);

            // Warmup, just in case
            Utilities.TestMethod(TestPathClosed, "ClosedPath", 50, false);

            float completeTime = 0;
            completeTime += Utilities.TestMethod(TestPathNear, "NearPath");
            completeTime += Utilities.TestMethod(TestPathOpen, "OpenPath");
            completeTime += Utilities.TestMethod(TestPathTypical, "TypicalPath");
            completeTime += Utilities.TestMethod(TestPathOpenReversed, "OpenPathReversed");
            completeTime += Utilities.TestMethod(TestPathMediumLength, "MediumLength");
            completeTime += Utilities.TestMethod(TestPathClosed, "ClosedPath");
            Debug.Log(completeTime.ToString("F5"));
        }

        private void TestPathOpen()
        {
            GetPath(new Vector2(-18.0f, 8.0f), new Vector2(18.0f, -9.0f), _testPath);
        }

        private void TestPathOpenReversed()
        {
            GetPath(new Vector2(18.0f, -9.0f), new Vector2(-18.0f, 8.0f), _testPath);
        }

        private void TestPathClosed()
        {
            GetPath(new Vector2(-18.0f, 8.0f), new Vector2(18.0f, -5.0f), _testPath);
        }

        private void TestPathTypical()
        {
            GetPath(new Vector2(-18.0f, 8.0f), new Vector2(18.0f, 8.0f), _testPath);
        }

        private void TestPathNear()
        {
            GetPath(new Vector2(-18.0f, 8.0f), new Vector2(-14.0f, 8.0f), _testPath);
        }

        private void TestPathMediumLength()
        {
            GetPath(new Vector2(0f, -7.0f), new Vector2(18.0f, -5.0f), _testPath);
        }

		#endregion

		#region Helper Methods

		protected void CheckFieldSizeY()
		{
			if (!_usesDifferentHeight) { _fieldSizeY = fieldSizeX; }
		}

		protected GridPosition GetGridPositionFromLevelPosition(Vector2 levelPos) => new GridPosition(GetColumnFromX(levelPos.x), GetRowFromY(levelPos.y));

		protected Vector2 GetLevelPositionFromGridPosition(GridPosition pos) => new Vector2(GetPositionFromColumn(pos.column), GetPositionFromRow(pos.row));

		protected int GetColumnFromX(float posX) => (int)((posX - _startX) / _fieldSizeX);

		protected int GetRowFromY(float posY) => (int)((posY - _startY) / _fieldSizeY);

		protected float GetPositionFromColumn(int row) => _startX + row * _fieldSizeX + _fieldSizeX * 0.5f;

		protected float GetPositionFromRow(int row) => _startY + row * _fieldSizeY + _fieldSizeY * 0.5f;

		protected void SetGridSize(float fieldSizeX, float fieldSizeY)
		{
			_usesDifferentHeight = true;
			_fieldSizeX = fieldSizeX;
			_fieldSizeY = fieldSizeY;
			_debugDrawSize = new Vector3(_fieldSizeX, _fieldSizeY, _fieldSizeX);
		}

        private void DrawMarker(int row, int column)
        {
            float posX = _startX + column * _fieldSizeX + _fieldSizeX * 0.5f;
            float posY = _startY + row * _fieldSizeY + _fieldSizeY * 0.5f;

            Vector3 center = new Vector3(posX, posY, 0);
            Gizmos.DrawCube(center, _debugDrawSize);
        }

        #endregion
    }
}