using UnityEngine;

namespace egads.system.camera
{
	public class CameraTargetThis : MonoBehaviour
	{
        #region Unity Methods

        private void Awake()
		{
			CameraSmoothFollow2D camera = FindObjectOfType<CameraSmoothFollow2D>();
			if (camera != null) { camera.target = transform; }
		}

        #endregion
    }
}