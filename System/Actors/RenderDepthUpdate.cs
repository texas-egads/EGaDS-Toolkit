using UnityEngine;

namespace egads.system.actors
{
	public class RenderDepthUpdate : MonoBehaviour
	{
        #region Public Properties

        public bool toBackgroundWhenDead = false;
		public float offset = 0f;

        #endregion

        #region Private Properties

        // State
        private bool _isActive = true;

		private Transform _transform;
		private Actor2D _actor;

        #endregion

        #region Unity Methods

        private void Awake()
		{
			_transform = transform;

			_actor = GetComponent<Actor2D>();
			if (_actor != null) { _actor.stateChanged += ActorStateChanged; }	
		}

		private void Update()
		{
			if (_isActive) { _transform.position = new Vector3(_transform.position.x, _transform.position.y, _transform.position.y * 0.1f + offset); }	
		}

		public Vector3 UpdatePosition(Vector3 current) => new Vector3(current.x, current.y, current.y * 0.1f + offset);

        #endregion

        #region Private Methods

        private void ActorStateChanged(IActor activeActor, ActorState state)
		{
			// Reset depth rendering when actor is active again
			if (activeActor.isAlive && !_isActive) { _isActive = true; }
				
		}

        #endregion
    }
}