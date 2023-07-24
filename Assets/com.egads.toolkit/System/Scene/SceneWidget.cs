using UnityEngine;
using egads.tools.objects;

namespace egads.system.scene
{
	public class SceneWidget : PooledObject
	{
        #region Private Properties

        [SerializeField]
		private Transform _target;
		private RectTransform _rectTransform;
		private RectTransform _canvasRect;

        #endregion

        #region Unity Methods

        protected virtual void Awake()
		{
			_rectTransform = GetComponent<RectTransform>();
			_canvasRect = transform.root.GetComponent<Canvas>().GetComponent<RectTransform>();
		}

		protected virtual void LateUpdate()
		{
			UpdatePosition();
		}

        #endregion

        #region Private Methods

        private void UpdatePosition()
		{
			if (_target != null)
			{
				// 0,0 for the canvas is at the center of the screen,
				// Whereas WorldToViewPortPoint treats the lower left corner as 0,0.
				// Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.

				Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(_target.position);
				Vector2 WorldObject_ScreenPosition = new Vector2(
				((ViewportPosition.x * _canvasRect.sizeDelta.x) - (_canvasRect.sizeDelta.x * 0.5f)),
				((ViewportPosition.y * _canvasRect.sizeDelta.y) - (_canvasRect.sizeDelta.y * 0.5f)));

				_rectTransform.anchoredPosition = WorldObject_ScreenPosition;
			}
		}

        #endregion
    }
}