using UnityEngine;

namespace egads.system.objectMovement
{
	public class StandardTrigger<T> : MonoBehaviour
	{
        #region Events

        public event System.Action<T> triggerWasEntered;

        #endregion

        #region Unity Methods

        public void OnTriggerEnter2D(Collider2D coll)
		{
			T hit = coll.transform.GetComponent<T>();

			if (hit != null && triggerWasEntered != null) { triggerWasEntered(hit); }
		}

        #endregion
    }
}