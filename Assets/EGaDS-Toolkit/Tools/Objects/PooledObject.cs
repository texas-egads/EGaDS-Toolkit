using UnityEngine;

namespace egads.tools.objects
{
	public class PooledObject : MonoBehaviour, IPooledObject
	{
        #region IPooledObject Implementation

        public event System.Action<IPooledObject> getsDisabled;
        public bool isUsedByObjectPool
        {
            get { return _isUseByObjectPool; }
            set { _isUseByObjectPool = value; }
        }


		public virtual void ToggleOn()
		{
			gameObject.SetActive(true);
			_isActive = true;
		}

		public virtual void ToggleOff()
		{
			StopAllCoroutines();
			gameObject.SetActive(false);
			_isActive = false;
		}

        #endregion

        #region Properties

        private bool _isActive = true;
        public bool isActive { get { return _isActive; } }
        private bool _isUseByObjectPool = false;

        #endregion

        #region Methods

        public virtual void NotifyDisabled()
		{
			if (isUsedByObjectPool)
			{
				if (getsDisabled != null) { getsDisabled(this); }
			}
			else { Destroy(gameObject); }
		}

        #endregion
    }
}