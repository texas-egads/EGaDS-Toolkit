using UnityEngine;

namespace egads.tools.extensions
{
	public static class Vector3Extensions
	{
        #region Methods

        public static void Invert(this Vector3 vector)
		{
			vector.x = -vector.x;
			vector.y = -vector.y;
			vector.z = -vector.z;
		}

		public static Vector2 Vector2(this Vector3 vector) => new Vector2(vector.x, vector.y);

        #endregion
    }
}