using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace egads.system.levelConnection
{
	public class LevelEntry : MonoBehaviour
	{
        #region Public Properites

        public LevelConnection incomingConnection;

		public bool lookToTheRight = true;
		public bool lookDown = true;

        #endregion

        #region Public Methods

        public Vector2 GetLookDirection()
		{
			if (lookToTheRight)
			{
				if (lookDown) { return new Vector2(1f, -1f); }
				else { return new Vector2(1f, 1f); }
			}
			else
			{
				if (lookDown) { return new Vector2(-1f, -1f); }
				else { return new Vector2(-1f, 1f); }
			}
		}

        #endregion
    }
}