using UnityEngine;

namespace egads.tools.utils
{
	public static class GizmosUtilities
	{
        #region Methods

        public static void DrawRect(Rect rect, Color color)
		{
			Gizmos.color = color;

			// Top line
			Vector3 firstPos = new Vector3(rect.xMin, rect.yMin, 0f);
			Vector3 secondPos = new Vector3(rect.xMax, rect.yMin, -1.0f);
			Gizmos.DrawLine(firstPos, secondPos);

			// Bottom line
			firstPos = new Vector3(rect.xMin, rect.yMax, 0f);
			secondPos = new Vector3(rect.xMax, rect.yMax, -1.0f);
			Gizmos.DrawLine(firstPos, secondPos);

			// Bottom line
			firstPos = new Vector3(rect.xMin, rect.yMin, 0f);
			secondPos = new Vector3(rect.xMin, rect.yMax, -1.0f);
			Gizmos.DrawLine(firstPos, secondPos);

			// Bottom line
			firstPos = new Vector3(rect.xMax, rect.yMin, 0f);
			secondPos = new Vector3(rect.xMax, rect.yMax, -1.0f);
			Gizmos.DrawLine(firstPos, secondPos);
		}

        #endregion
    }
}