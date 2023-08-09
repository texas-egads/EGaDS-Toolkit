using UnityEngine;
using egads.system.gameManagement;

namespace egads.system.camera
{
    /// <summary>
    /// Handles camera shaking functionality.
    /// </summary>
    public class CameraShake : MonoBehaviour
    {
        #region Private Properties

        // The intensity of the camera shake.
        [SerializeField]
        private float _shakeAmount = 0.25f;
        // The rate at which the shake decreases over time.
        [SerializeField]
        private float _decreaseFactor = 1.0f;

        // Reference to the camera component.
        private Camera _camera;
        // The camera's original position before shaking.
        private Vector3 _originalPos;

        // Current shake intensity.
        private float _shake = 0.0f; 

        #endregion

        #region Unity Methods

        private void Awake()
        {
            // Get the Camera component on Awake.
            _camera = GetComponent<Camera>(); 
        }

        private void Update()
        {
            // Check if the game is running or in a sequence, and if time is not paused.
            if (MainBase.isRunningOrInSequence && Time.timeScale > 0)
            {
                if (_shake > 0.0f)
                {
                    // Generate a random shake position and apply it to the camera's local position.
                    Vector2 shakePos = Random.insideUnitCircle * _shakeAmount * _shake;
                    _camera.transform.localPosition = new Vector3(shakePos.x, shakePos.y, _originalPos.z);

                    // Decrease the shake intensity over time.
                    _shake -= Time.deltaTime * _decreaseFactor;

                    // Reset the camera's position when shake intensity reaches zero.
                    if (_shake <= 0.0f)
                    {
                        _shake = 0.0f;
                        _camera.transform.localPosition = _originalPos;
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initiates camera shaking with a given intensity.
        /// </summary>
        /// <param name="amount">The intensity of the shake.</param>
        public void Shake(float amount)
        {
            if (_shake <= 0.0f) { _originalPos = _camera.transform.localPosition; }

            // Set the shake intensity to the specified amount.
            _shake = amount; 
        }

        #endregion
    }
}
