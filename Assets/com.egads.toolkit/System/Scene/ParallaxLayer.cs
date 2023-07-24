using UnityEngine;

namespace egads.system.scene
{
	public class ParallaxLayer : MonoBehaviour
	{
        #region Parallax Direction

        public enum ParallaxDirection
		{
			Horizontal,
			Vertical,
			Both
		}
        public ParallaxDirection direction = ParallaxDirection.Horizontal;

        #endregion

        #region Camera Properties

        // 0 -> Exactly as camera
        // 1 -> Staying exactly in the background
        public float scaleHorizontal;
		public float scaleVertical;

		private Transform _cameraTransform;
		private Transform _transform;

		private float _cameraStartX;
		private float _layerStartX;
		private float _cameraStartY;
		private float _layerStartY;

        #endregion

        #region Unity Methods

        private void Awake()
		{
			_cameraTransform = Camera.main.transform;
			_transform = transform;

			_cameraStartX = _cameraTransform.position.x;
			_cameraStartY = _cameraTransform.position.y;
			_layerStartX = _transform.position.x;
			_layerStartY = _transform.position.y;
		}

		private void LateUpdate()
		{
			UpdatePosition();
		}

        #endregion

        #region Private Methods

        private void UpdatePosition()
		{
			Vector3 newPos = _transform.position;

			if (direction == ParallaxDirection.Horizontal || direction == ParallaxDirection.Both)
			{
				float hDistance = _cameraTransform.position.x - _cameraStartX;
				newPos.x = _layerStartX + hDistance * scaleHorizontal;
			}

			if (direction == ParallaxDirection.Vertical || direction == ParallaxDirection.Both)
			{
				float vDistance = _cameraTransform.position.y - _cameraStartY;
				newPos.y = _layerStartY + vDistance * scaleVertical;
			}

			_transform.position = newPos;
		}

        #endregion
    }
}