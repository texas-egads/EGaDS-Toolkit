using UnityEngine;
using egads.system.gameManagement;

namespace egads.system.camera
{
	public class CameraShake : MonoBehaviour
	{
        #region Private Properties

        [SerializeField]
		private float _shakeAmount = 0.25f;
		[SerializeField]
		private float _decreaseFactor = 1.0f;

		private Camera _camera;
		private Vector3 _originalPos;
		private float _shake = 0.0f;

        #endregion

        #region Unity Methods

        private void Awake()
		{
			_camera = GetComponent<Camera>();
		}

		private void Update()
		{
			if (MainBase.isRunningOrInSequence && Time.timeScale > 0)
			{
				if (_shake > 0.0f)
				{
					Vector2 shakePos = Random.insideUnitCircle * _shakeAmount * _shake;
					_camera.transform.localPosition = new Vector3(shakePos.x, shakePos.y, _originalPos.z);

					_shake -= Time.deltaTime * _decreaseFactor;

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

		public void Shake(float amount)
		{
			if (_shake <= 0.0f) { _originalPos = _camera.transform.localPosition; }
			_shake = amount;
		}

        #endregion
    }
}