using UnityEngine;

namespace egads.system.objectMovement
{
	public class HorizontalFollow : MonoBehaviour
	{
        #region Follow Properties

        public Transform target;

		private Transform _transform;

        #endregion

        #region Unity Methods

        void Awake()
		{
			_transform = transform;
		}

		void Update()
		{
			if (target != null) { _transform.position = new Vector3(target.position.x, _transform.position.y, _transform.position.z); }
		}

        #endregion
    }
}