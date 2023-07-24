﻿using UnityEngine;
using System.Collections.Generic;

namespace egads.system.objectMovement
{
	public class FloatingObjectsController : MonoBehaviour
	{
        #region Rect Properties

        public Rect bounds = new Rect(-10, -10, 20, 20);

		private Rect _levelBounds;
		public Rect levelBounds
		{
			get
			{
				CalculateBounds();
				return _levelBounds;
			}
		}

		// Display in editor window
		public bool drawRectangle = true;
		public Color lineColor = Color.blue;

        #endregion

        #region Private Properties

        private Transform _transform;
		private List<FloatingObject2D> children;

        #endregion

        #region Unity Methods

        void Awake()
		{
			_transform = transform;

			children = new List<FloatingObject2D>(_transform.GetComponentsInChildren<FloatingObject2D>());

			CalculateBounds();
		}

		void Update()
		{
			for (int i = 0; i < children.Count; i++) { UpdateFloatingObject(children[i]); }
		}

		void UpdateFloatingObject(FloatingObject2D item)
		{
			Vector3 movement = item.movement * Time.deltaTime;
			item.objectTransform.position += movement;

			Vector3 newPos = item.objectTransform.position;
			if (newPos.x < _levelBounds.x) { newPos.x = _levelBounds.xMax; }
			if (newPos.y < _levelBounds.y) { newPos.y = _levelBounds.yMax; }
			if (newPos.x > _levelBounds.xMax) { newPos.x = _levelBounds.x; }
			if (newPos.y > _levelBounds.yMax) { newPos.y = _levelBounds.y; }

			item.objectTransform.position = newPos;
		}

		void OnDrawGizmos()
		{
			Gizmos.color = lineColor;

			CalculateBounds();

			// Horizontal lines
			if (drawRectangle)
			{
				// Top line
				Vector3 firstPos = new Vector3(_levelBounds.xMin, _levelBounds.yMin, 0f);
				Vector3 secondPos = new Vector3(_levelBounds.xMax, _levelBounds.yMin, -1.0f);
				Gizmos.DrawLine(firstPos, secondPos);

				// Bottom line
				firstPos = new Vector3(_levelBounds.xMin, _levelBounds.yMax, 0f);
				secondPos = new Vector3(_levelBounds.xMax, _levelBounds.yMax, -1.0f);
				Gizmos.DrawLine(firstPos, secondPos);

				// Bottom line
				firstPos = new Vector3(_levelBounds.xMin, _levelBounds.yMin, 0f);
				secondPos = new Vector3(_levelBounds.xMin, _levelBounds.yMax, -1.0f);
				Gizmos.DrawLine(firstPos, secondPos);

				// Bottom line
				firstPos = new Vector3(_levelBounds.xMax, _levelBounds.yMin, 0f);
				secondPos = new Vector3(_levelBounds.xMax, _levelBounds.yMax, -1.0f);
				Gizmos.DrawLine(firstPos, secondPos);
			}
		}

        #endregion

        #region Private Methods

        /// <summary>
        /// Calculates level bounds from position of this object + bounds
        /// </summary>
        private void CalculateBounds()
		{
			_transform = transform;

			_levelBounds.xMin = _transform.position.x + bounds.xMin;
			_levelBounds.yMin = _transform.position.y + bounds.yMin;
			_levelBounds.width = bounds.width;
			_levelBounds.height = bounds.height;
		}

        #endregion
    }
}