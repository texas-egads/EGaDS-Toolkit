using UnityEngine;

namespace egads.tools.extensions
{
	public static class Vector2Extensions
	{
        #region Methods

        public static void Invert(this Vector2 vector)
		{
			vector.x = -vector.x;
			vector.y = -vector.y;
		}

		public static Vector3 Vector3(this Vector2 vector) => new Vector3(vector.x, vector.y, 0);

        #endregion
    }
}