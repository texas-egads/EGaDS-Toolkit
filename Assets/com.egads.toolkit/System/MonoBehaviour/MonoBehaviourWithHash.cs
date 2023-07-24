using UnityEngine;
using UnityEngine.SceneManagement;
using egads.system.gameManagement;

namespace egads.system.monoBehaviour
{
	public class MonoBehaviourWithHash : MonoBehaviour
	{
        #region Hash Properties

        private int? _hash;
		public int hash
		{
			get
			{
				if (_hash == null) { _hash = transform.position.GetHashCode() + SceneManager.GetActiveScene().name.GetHashCode(); }
					
				return _hash.Value;
			}
		}
		public bool hashWasEntered => MainBase.Instance.gameStateData.wasUsed.Contains(hash);

        #endregion

        #region Hash Methods

        public void EnterHash()
		{
			MainBase.Instance.gameStateData.wasUsed.Add(hash);
		}

		public void RemoveHash()
		{
			if (hashWasEntered) { MainBase.Instance.gameStateData.wasUsed.Remove(hash); }
		}

        #endregion
    }
}