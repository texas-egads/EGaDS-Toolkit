using UnityEngine;

namespace egads.ssytem.UI
{
	public class IgnoreRaycast : MonoBehaviour, ICanvasRaycastFilter
	{
        #region Public Methods

        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
		{
			return false;
		}

        #endregion
    }
}
