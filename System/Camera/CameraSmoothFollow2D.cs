using UnityEngine;
using UnityEngine.SceneManagement;
using egads.tools.utils;
using egads.tools.extensions;
using egads.system.pathFinding;
using System.Collections.Generic;

namespace egads.system.camera
{
    #region Move Direction

    /// <summary>
    /// Enumeration representing the available movement directions for the camera.
    /// </summary>
    public enum MoveDirection
    {
        /// <summary>
        /// Camera can move both horizontally and vertically.
        /// </summary>
        HorizontalAndVertical,

        /// <summary>
        /// Camera can only move horizontally.
        /// </summary>
        Horizontal,

        /// <summary>
        /// Camera can only move vertically.
        /// </summary>
        Vertical
    }

    #endregion

    public class CameraSmoothFollow2D : MonoBehaviour
	{
        #region Target Properties

        // Optional target for single target following
        private Transform _target = null;

        /// <summary>
        /// Gets or sets the single target for the camera to follow.
        /// </summary>
        public Transform target
        {
            get { return _target; }
            set
            {
                _targets = null; // Clear the list of targets
                _target = value; // Set the new single target
            }
        }

        // List of targets for multiple target following
        private List<Transform> _targets = null;

        /// <summary>
        /// Gets or sets the list of targets for the camera to follow.
        /// </summary>
        public List<Transform> targets
        {
            get { return _targets; }
            set
            {
                target = null; // Clear the single target
                _targets = value; // Set the new list of targets
            }
        }

        /// <summary>
        /// Returns whether the camera has a target or targets to follow.
        /// </summary>
        public bool hasTarget => (_targets != null || _target != null);

        #endregion

        #region Public Properties

        [SerializeField]
        /// <summary>
        /// The direction in which the camera follows the target(s).
        /// </summary>
        public MoveDirection direction = MoveDirection.HorizontalAndVertical;

        /// <summary>
        /// The horizontal speed at which the camera follows the target(s).
        /// </summary>
        public float horizontalFollowSpeed = 2.0f;

        /// <summary>
        /// The vertical speed at which the camera follows the target(s).
        /// </summary>
        public float verticalFollowSpeed = 2.0f;

        /// <summary>
        /// The distance from the screen edges where the camera movement becomes smooth.
        /// </summary>
        public float smoothEdge = 2.0f;

        /// <summary>
        /// The minimum size of the orthographic camera view.
        /// </summary>
        public float minimumOrthoGraphicSize = 1.0f;

        /// <summary>
        /// The speed at which the camera zooms in or out.
        /// </summary>
        public float zoomSpeed = 5f;

        /// <summary>
        /// Indicates whether the camera can move upwards.
        /// </summary>
        public bool canMoveUp => _canMoveUp;

        /// <summary>
        /// Indicates whether the camera can move downwards.
        /// </summary>
        public bool canMoveDown => _canMoveDown;

        /// <summary>
        /// Indicates whether the camera can move to the left.
        /// </summary>
        public bool canMoveLeft => _canMoveLeft;

        /// <summary>
        /// Indicates whether the camera can move to the right.
        /// </summary>
        public bool canMoveRight => _canMoveRight;

        #endregion

        #region Private Properties

        // The initial horizontal follow speed before any modifications.
        private float _horizontalFollowSpeedStart = 2.0f;

        // The initial vertical follow speed before any modifications.
        private float _verticalFollowSpeedStart = 2.0f;

        // Flags indicating whether the camera can move in different directions.
        private bool _canMoveUp = false;
        private bool _canMoveDown = false;
        private bool _canMoveLeft = false;
        private bool _canMoveRight = false;

        // Flag indicating whether to check for level boundaries.
        private bool _checkForBounds = false;

        // The outer boundaries of the level.
        private Rect _bounds;

        // The inner boundaries of the level, adjusted for smooth camera movement.
        private Rect _innerBounds;

        #endregion

        #region Camera Properties

        // Reference to the Camera component.
        protected Camera _camera;

        /// <summary>
        /// Gets the Camera component. Initializes it if not already set.
        /// </summary>
        public Camera Camera
        {
            get
            {
                if (_camera == null) { Init(); }
                return _camera;
            }
        }

        // Reference to the transform of the camera.
        protected Transform _transform;

        // Reference to the CameraShake component attached to the camera.
        protected CameraShake _cameraShake;

        /// <summary>
        /// Gets the CameraShake component attached to the camera.
        /// </summary>
        public CameraShake shaker => _cameraShake;

        // The target orthographic size of the camera.
        private float _targetOrthoSize;

        #endregion

        #region Unity Methods

        /// <summary>
        /// Initializes camera properties and sets initial values on Awake.
        /// </summary>
        protected void Awake()
        {
            Init();

            _horizontalFollowSpeedStart = horizontalFollowSpeed;
            _verticalFollowSpeedStart = verticalFollowSpeed;
            _targetOrthoSize = _camera.orthographicSize;

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        /// <summary>
        /// Handles initialization when a new scene is loaded.
        /// </summary>
        /// <param name="scene">The loaded scene.</param>
        /// <param name="mode">The loading mode.</param>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Init();
        }

        /// <summary>
        /// LateUpdate is called after all Update calls have been made. Updates camera properties and follows the target.
        /// </summary>
        protected void LateUpdate()
        {
            // Update zooming
            if (_camera.orthographicSize != _targetOrthoSize)
            {
                _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _targetOrthoSize, Time.deltaTime * zoomSpeed);
                if (Mathf.Abs(_camera.orthographicSize - _targetOrthoSize) < 0.001f)
                    _camera.orthographicSize = _targetOrthoSize;
            }

            if (hasTarget) { FollowTarget(); }
            else if (_checkForBounds) { ApplyBounds(); }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Zooms the camera by a specified amount.
        /// </summary>
        /// <param name="delta">The amount to zoom by.</param>
        public void Zoom(float delta)
		{
			_camera.orthographicSize += delta;

			ApplyBounds();
		}

        /// <summary>
        /// Sets the orthographic size of the camera while keeping a pivot point at a certain position.
        /// </summary>
        /// <param name="size">The new orthographic size.</param>
        /// <param name="pivot">The pivot point in world space.</param>
        /// <param name="directionToCamera">The direction vector from the pivot to the camera.</param>
        /// <param name="oldSize">The previous orthographic size.</param>
        public void SetOrthgraphicSizeAtPivot(float size, Vector2 pivot, Vector2 directionToCamera, float oldSize)
		{
			if (size < minimumOrthoGraphicSize && _camera.orthographicSize <= minimumOrthoGraphicSize) { return; }

			if (size < minimumOrthoGraphicSize) { size = minimumOrthoGraphicSize; }

			_camera.orthographicSize = size;

			float scale = size / oldSize;

			Vector3 newPosition = pivot + directionToCamera * scale;
			_camera.transform.position = new Vector3(newPosition.x, newPosition.y, _camera.transform.position.z);

			ApplyBounds();
		}

        /// <summary>
        /// Translates the camera by a specified delta in screen space.
        /// </summary>
        /// <param name="delta">The translation delta in screen space.</param>
        public void Translate(Vector2 delta)
		{
			float d = Screen.height / (_camera.orthographicSize * 2.0f);
			float y = delta.y / d;
			float x = delta.x / d;

			_transform.Translate(x, y, 0);

			ApplyBounds();
		}

        /// <summary>
        /// Sets the position of the camera.
        /// </summary>
        /// <param name="pos">The new position in world space.</param>
        public void SetPosition(Vector2 pos)
		{
			_transform.position = new Vector3(pos.x, pos.y, _transform.position.z);

			ApplyBounds();
		}

        /// <summary>
        /// Sets the level boundaries for the camera movement.
        /// </summary>
        /// <param name="levelBoundaries">The level boundaries information.</param>
        public void SetBoundaries(LevelBoundaries levelBoundaries)
		{
			if (levelBoundaries != null)
			{
				_bounds = levelBoundaries.levelBounds;
				_innerBounds = new Rect(_bounds.x + smoothEdge, _bounds.y + smoothEdge, _bounds.width - 2 * smoothEdge, _bounds.height - 2 * smoothEdge);

				_checkForBounds = true;

				ApplyBounds();
			}
			else { _checkForBounds = false; }
		}

        /// <summary>
        /// Sets the orthographic size of the camera and applies level boundaries.
        /// </summary>
        /// <param name="size">The new orthographic size.</param>
        public void SetOrthoSize(float size)
		{
			_targetOrthoSize = size;
			_camera.orthographicSize = size;

			ApplyBounds();
		}

        /// <summary>
        /// Sets the target orthographic size without applying level boundaries.
        /// </summary>
        /// <param name="size">The new target orthographic size.</param>
        public void SetTargetOrthoSize(float size)
		{
			_targetOrthoSize = size;
		}

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes camera-related properties and components.
        /// </summary>
        private void Init()
        {
            _camera = GetComponentInChildren<Camera>();
            _cameraShake = GetComponentInChildren<CameraShake>();
            _transform = transform;

            // Find and set level boundaries if available.
            LevelBoundaries levelBoundaries = FindObjectOfType<LevelBoundaries>();
            SetBoundaries(levelBoundaries);
        }

        /// <summary>
        /// Follows the target's position while considering level boundaries.
        /// </summary>
        protected void FollowTarget()
		{
            // Calculate camera boundary and scaling factors.
            Rect cameraRect = Utilities2D.CameraBounds2D(_camera);
            float horizontalScale = 1.0f;
            float verticalScale = 1.0f;

            // Get the target position to follow.
            Vector3 targetPos = GetTargetPosition();
            if (float.IsNaN(targetPos.x) || float.IsNaN(targetPos.y)) { return; }

            if (_checkForBounds)
			{
				// Left smooth edge
				if (targetPos.x < _transform.position.x	&& cameraRect.x > _bounds.x && cameraRect.x < _innerBounds.x) { horizontalScale = 1.0f + (cameraRect.x - _innerBounds.x) / smoothEdge; }

				// Right smooth edge
				else if (targetPos.x > _transform.position.x && cameraRect.xMax > _innerBounds.xMax && cameraRect.xMax < _bounds.xMax) { horizontalScale = 1.0f - (cameraRect.xMax - _innerBounds.xMax) / smoothEdge; }

				// Hard left edge
				else if (targetPos.x < _transform.position.x && cameraRect.x <= _bounds.x) { horizontalScale = 0; }

				// Hard right edge
				else if (targetPos.x > _transform.position.x && cameraRect.xMax >= _bounds.xMax) { horizontalScale = 0; }

				// Bottom edge
				if (targetPos.y < _transform.position.y	&& cameraRect.y > _bounds.y && cameraRect.y < _innerBounds.y) { verticalScale = 1.0f + (cameraRect.y - _innerBounds.y) / smoothEdge; }

				// Top edge
				else if (targetPos.y > _transform.position.y && cameraRect.yMax > _innerBounds.yMax && cameraRect.yMax < _bounds.yMax) { verticalScale = 1.0f - (cameraRect.yMax - _innerBounds.yMax) / smoothEdge; }

				// Hard bottom edge
				else if (targetPos.y < _transform.position.y && cameraRect.y <= _bounds.y) { verticalScale = 0; }

				// Hard top edge
				else if (targetPos.y > _transform.position.y && cameraRect.yMax >= _bounds.yMax) { verticalScale = 0; }
			}

            // Calculate new camera position based on target position and scaling factors.
            float targetX = Mathf.Lerp(_transform.position.x, targetPos.x, horizontalFollowSpeed * Time.deltaTime * horizontalScale);
            if (direction == MoveDirection.Vertical) { targetX = _transform.position.x; }

            float targetY = Mathf.Lerp(_transform.position.y, targetPos.y, verticalFollowSpeed * Time.deltaTime * verticalScale);
            if (direction == MoveDirection.Horizontal) { targetY = _transform.position.y; }

            // Apply the calculated position to the camera transform.
            _transform.position = new Vector3(targetX, targetY, _transform.position.z);

            ApplyBounds();
        }

        /// <summary>
        /// Gets the position of the target. If multiple targets are available, gets their center position.
        /// </summary>
        /// <returns>The position of the target or the center position of multiple targets.</returns>
        private Vector3 GetTargetPosition()
		{
			if (_target != null) { return _target.position; }

			return _targets.GetCenter();
		}

        /// <summary>
        /// Applies level boundaries to the camera's position and orthographic size.
        /// </summary>
        protected void ApplyBounds()
		{
			if (!_checkForBounds) { return; }

            // Calculate camera boundary.
            Rect cameraRect = Utilities2D.CameraBounds2D(_camera);

            // Reset movement flags.
            _canMoveRight = true;
            _canMoveLeft = true;
            _canMoveDown = true;
            _canMoveUp = true;

            // Scaling
            if (_camera.orthographicSize < minimumOrthoGraphicSize)
			{
				_camera.orthographicSize = minimumOrthoGraphicSize;
				cameraRect = Utilities2D.CameraBounds2D(_camera);
			}

			if (cameraRect.width > _bounds.width)
			{
				float resize = _bounds.width / cameraRect.width;
				_camera.orthographicSize *= resize;
				cameraRect = Utilities2D.CameraBounds2D(_camera);
			}

			if (cameraRect.height > _bounds.height)
			{
				float resize = _bounds.height / cameraRect.height;
				_camera.orthographicSize *= resize;
				cameraRect = Utilities2D.CameraBounds2D(_camera);
			}

			ApplyHardBounds(cameraRect);
		}

        /// <summary>
        /// Applies hard bounds to the camera's position based on the defined level boundaries.
        /// </summary>
        /// <param name="cameraRect">The current camera boundary.</param>
        /// <returns>The updated camera boundary.</returns>
        private Rect ApplyHardBounds(Rect cameraRect)
		{
            // A small error margin to prevent undesired clipping.
            float errorMargin = 0.001f;

            // Adjustments for hard boundary constraints
            if (cameraRect.xMin <= _bounds.xMin - errorMargin)
			{
				_transform.position += new Vector3(_bounds.xMin - cameraRect.xMin, 0, 0);
				cameraRect = Utilities2D.CameraBounds2D(_camera);
				_canMoveLeft = false;
			}

			if (cameraRect.yMin <= _bounds.yMin - errorMargin)
			{
				_transform.position += new Vector3(0, _bounds.yMin - cameraRect.yMin, 0);
				cameraRect = Utilities2D.CameraBounds2D(_camera);
				_canMoveDown = false;
			}

			if (cameraRect.xMax >= _bounds.xMax + errorMargin)
			{
				_transform.position -= new Vector3(cameraRect.xMax - _bounds.xMax, 0, 0);
				cameraRect = Utilities2D.CameraBounds2D(_camera);
				_canMoveRight = false;
			}

			if (cameraRect.yMax >= _bounds.yMax + errorMargin)
			{
				_transform.position -= new Vector3(0, cameraRect.yMax - _bounds.yMax, 0);
				cameraRect = Utilities2D.CameraBounds2D(_camera);
				_canMoveUp = false;
			}

			return cameraRect;
		}

        #endregion

        #region Follow Speed Methods

        /// <summary>
        /// Scales the horizontal and vertical follow speeds by a specified factor.
        /// </summary>
        /// <param name="scale">The factor by which to scale the follow speeds.</param>
        public void ScaleFollowSpeed(float scale)
        {
            horizontalFollowSpeed = _horizontalFollowSpeedStart * scale;
            verticalFollowSpeed = _verticalFollowSpeedStart * scale;
        }

        /// <summary>
        /// Resets the horizontal and vertical follow speeds to their original values.
        /// </summary>
        public void ResetFollowSpeed()
        {
            horizontalFollowSpeed = _horizontalFollowSpeedStart;
            verticalFollowSpeed = _verticalFollowSpeedStart;
        }

        #endregion
    }
}
