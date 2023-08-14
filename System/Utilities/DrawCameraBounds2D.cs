using UnityEngine;

namespace egads.system.utilities
{
	/// <summary>
	/// Editor script for visualizing camera bounds with 2D / orthographic camera
	/// </summary>
	public class DrawCameraBounds2D : MonoBehaviour
	{
		#region Public Properties

		public bool drawRectangle = true;
		public bool drawHorizontalLines;
		public bool drawVerticalLines;

		public Color lineColor = Color.red;

        #endregion

        #region Private Properties

        private float longDistance = 10000.0f;

        #endregion

        #region Unity Editor Methods

        private void OnDrawGizmos()
		{
			Transform transform = this.transform;
			Camera camera = GetComponentInChildren<Camera>();

			if (camera == null) { return; }

			Gizmos.color = lineColor;

			float vDistance = camera.orthographicSize;	// orthographic size is half of camera height
			float hDistance = camera.aspect * camera.orthographicSize;

			// Horizontal lines
			if (drawHorizontalLines || drawRectangle)
			{
				float h = longDistance;
				if (!drawHorizontalLines) { h = hDistance; }

				// Draw bottom line
				Vector3 leftPos = new Vector3(transform.position.x - h, transform.position.y - vDistance, -1.0f);
				Vector3 rightPos = new Vector3(transform.position.x + h, transform.position.y - vDistance, -1.0f);
				Gizmos.DrawLine(leftPos, rightPos);

				// Draw top line
				leftPos = new Vector3(transform.position.x - h, transform.position.y + vDistance, -1.0f);
				rightPos = new Vector3(transform.position.x + h, transform.position.y + vDistance, -1.0f);
				Gizmos.DrawLine(leftPos, rightPos);
			}

			// Vertical lines
			if (drawVerticalLines || drawRectangle)
			{
				float v = longDistance;
				if (!drawVerticalLines) { v = vDistance; }

				// Draw left line
				Vector3 leftPos = new Vector3(transform.position.x - hDistance, transform.position.y - v, -1.0f);
				Vector3 rightPos = new Vector3(transform.position.x - hDistance, transform.position.y + v, -1.0f);
				Gizmos.DrawLine(leftPos, rightPos);

				// Draw right line
				leftPos = new Vector3(transform.position.x + hDistance, transform.position.y - v, -1.0f);
				rightPos = new Vector3(transform.position.x + hDistance, transform.position.y + v, -1.0f);
				Gizmos.DrawLine(leftPos, rightPos);
			}
		}

        #endregion
    }
}
