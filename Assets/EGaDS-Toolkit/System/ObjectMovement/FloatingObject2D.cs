using UnityEngine;

namespace egads.system.objectMovement
{
	public class FloatingObject2D : MonoBehaviour
	{
        #region Public Properties

        public Vector2 movement;
		public Transform objectTransform;

        #endregion

        #region Unity Methods

        void Awake()
		{
			objectTransform = transform;
		}

        #endregion
    }
}