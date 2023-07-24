﻿using UnityEngine;
using UnityEngine.SceneManagement;
using egads.tools.utils;
using egads.tools.extensions;
using egads.system.pathFinding;
using System.Collections.Generic;

namespace egads.system.camera
{
    #region Move Direction

    public enum MoveDirection
	{
		HorizontalAndVertical,
		Horizontal,
		Vertical
	}

    #endregion

    public class CameraSmoothFollow2D : MonoBehaviour
	{
        #region Target Properties

        // Optional target
        private Transform _target = null;
		public Transform target
		{
			get { return _target; }
			set
			{
				_targets = null;
				_target = value;
			}
		}
		private List<Transform> _targets = null;
		public List<Transform> targets
		{
			get { return _targets; }
			set
			{
				target = null;
				_targets = value;
			}
		}
		public bool hasTarget => (_targets != null || _target != null);

        #endregion

        #region Public Properties

        [SerializeField]
		public MoveDirection direction = MoveDirection.HorizontalAndVertical;

		// Speed when following target
		public float horizontalFollowSpeed = 2.0f;
		public float verticalFollowSpeed = 2.0f;

        public float smoothEdge = 2.0f;

        public float minimumOrthoGraphicSize = 1.0f;
        public float zoomSpeed = 5f;

        public bool canMoveUp => _canMoveUp;
		public bool canMoveDown => _canMoveDown;
		public bool canMoveLeft => _canMoveLeft;
		public bool canMoveRight => _canMoveRight;

        #endregion

        #region Private Properties

        private float _horizontalFollowSpeedStart = 2.0f;
		private float _verticalFollowSpeedStart = 2.0f;

		private bool _canMoveUp = false;
		private bool _canMoveDown = false;
		private bool _canMoveLeft = false;
		private bool _canMoveRight = false;

		// Level boundaries
		private bool _checkForBounds = false;
		private Rect _bounds;
		private Rect _innerBounds;

        #endregion

        #region Camera Properties

        protected Camera _camera;
		public Camera Camera {
			get {
				if (_camera == null) { Init(); }
				return _camera;
			}
		}

		protected Transform _transform;

		protected CameraShake _cameraShake;
		public CameraShake shaker => _cameraShake;
		private float _targetOrthoSize;

        #endregion

        #region Unity Methods

        protected void Awake()
		{
			Init();

			_horizontalFollowSpeedStart = horizontalFollowSpeed;
			_verticalFollowSpeedStart = verticalFollowSpeed;
			_targetOrthoSize = _camera.orthographicSize;

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Init();
        }

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

        public void Zoom(float delta)
		{
			_camera.orthographicSize += delta;

			ApplyBounds();
		}

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

		public void Translate(Vector2 delta)
		{
			float d = Screen.height / (_camera.orthographicSize * 2.0f);
			float y = delta.y / d;
			float x = delta.x / d;

			_transform.Translate(x, y, 0);

			ApplyBounds();
		}

		public void SetPosition(Vector2 pos)
		{
			_transform.position = new Vector3(pos.x, pos.y, _transform.position.z);

			ApplyBounds();
		}

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

		public void SetOrthoSize(float size)
		{
			_targetOrthoSize = size;
			_camera.orthographicSize = size;

			ApplyBounds();
		}

		public void SetTargetOrthoSize(float size)
		{
			_targetOrthoSize = size;
		}

        #endregion

        #region Private Methods

        private void Init()
		{
			_camera = GetComponentInChildren<Camera>();
			_cameraShake = GetComponentInChildren<CameraShake>();
			_transform = transform;

			LevelBoundaries levelBoundaries = FindObjectOfType<LevelBoundaries>();
			SetBoundaries(levelBoundaries);
		}

		protected void FollowTarget()
		{
			Rect cameraRect = Utilities2D.CameraBounds2D(_camera);
			float horizontalScale = 1.0f;
			float verticalScale = 1.0f;

			Vector3 targetPos = GetTargetPosition();
			if (float.IsNaN(targetPos.x) || float.IsNaN(targetPos.y)) { return; }

			if (_checkForBounds)
			{
				// Left smooth edge
				if (targetPos.x < _transform.position.x	&& cameraRect.x > _bounds.x && cameraRect.x < _innerBounds.x)
				{
					horizontalScale = 1.0f + (cameraRect.x - _innerBounds.x) / smoothEdge;
				}
				// Right smooth edge
				else if (targetPos.x > _transform.position.x && cameraRect.xMax > _innerBounds.xMax && cameraRect.xMax < _bounds.xMax)
				{
					horizontalScale = 1.0f - (cameraRect.xMax - _innerBounds.xMax) / smoothEdge;
				}
				// Hard left edge
				else if (targetPos.x < _transform.position.x && cameraRect.x <= _bounds.x)
				{
					horizontalScale = 0;
				}
				// Hard right edge
				else if (targetPos.x > _transform.position.x && cameraRect.xMax >= _bounds.xMax)
				{
					horizontalScale = 0;
				}

				// Bottom edge
				if (targetPos.y < _transform.position.y	&& cameraRect.y > _bounds.y && cameraRect.y < _innerBounds.y)
				{
					verticalScale = 1.0f + (cameraRect.y - _innerBounds.y) / smoothEdge;
				}
				// Top edge
				else if (targetPos.y > _transform.position.y && cameraRect.yMax > _innerBounds.yMax && cameraRect.yMax < _bounds.yMax)
				{
					verticalScale = 1.0f - (cameraRect.yMax - _innerBounds.yMax) / smoothEdge;
				}
				// Hard bottom edge
				else if (targetPos.y < _transform.position.y && cameraRect.y <= _bounds.y)
				{
					verticalScale = 0;
				}
				// Hard top edge
				else if (targetPos.y > _transform.position.y && cameraRect.yMax >= _bounds.yMax)
				{
					verticalScale = 0;
				}
			}

			float targetX = Mathf.Lerp(_transform.position.x, targetPos.x, horizontalFollowSpeed * Time.deltaTime * horizontalScale);
			if (direction == MoveDirection.Vertical) { targetX = _transform.position.x; }

			float targetY = Mathf.Lerp(_transform.position.y, targetPos.y, verticalFollowSpeed * Time.deltaTime * verticalScale);
			if (direction == MoveDirection.Horizontal) { targetY = _transform.position.y; }

			_transform.position = new Vector3(targetX, targetY, _transform.position.z);

			ApplyBounds();
		}

		private Vector3 GetTargetPosition()
		{
			if (_target != null) { return _target.position; }

			return _targets.GetCenter();
		}

		protected void ApplyBounds()
		{
			if (!_checkForBounds) { return; }

			Rect cameraRect = Utilities2D.CameraBounds2D(_camera);

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

		private Rect ApplyHardBounds(Rect cameraRect)
		{
			float errorMargin = 0.001f;

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

        public void ScaleFollowSpeed(float scale)
		{
			horizontalFollowSpeed = _horizontalFollowSpeedStart * scale;
			verticalFollowSpeed = _verticalFollowSpeedStart * scale;
		}

		public void ResetFollowSpeed()
		{
			horizontalFollowSpeed = _horizontalFollowSpeedStart;
			verticalFollowSpeed = _verticalFollowSpeedStart;
		}

        #endregion
    }
}
