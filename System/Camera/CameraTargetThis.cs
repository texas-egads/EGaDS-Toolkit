using UnityEngine;

namespace egads.system.camera
{
    /// <summary>
    /// Attaches this script to a GameObject to make it the target of a CameraSmoothFollow2D camera.
    /// </summary>
    public class CameraTargetThis : MonoBehaviour
    {
        #region Unity Methods

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Finds a CameraSmoothFollow2D component in the scene and assigns this transform as its target.
        /// </summary>
        private void Awake()
        {
            // Find the CameraSmoothFollow2D component in the scene.
            CameraSmoothFollow2D camera = FindObjectOfType<CameraSmoothFollow2D>();

            // Assign this transform as the target to the camera if it's found.
            if (camera != null)
            {
                camera.target = transform;
            }
        }

        #endregion
    }
}
