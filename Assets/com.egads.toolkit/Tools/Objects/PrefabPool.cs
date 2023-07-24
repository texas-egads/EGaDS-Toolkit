using UnityEngine;
using System.Collections.Generic;

namespace egads.tools.objects
{
	public class PrefabPool
	{
		#region Properties

		private Dictionary<MonoBehaviour, GameObjectPool> _prefabPool = new Dictionary<MonoBehaviour, GameObjectPool>();

		#endregion

		#region Methods

		// Get an Object from the pool
		public IPooledObject Pop(MonoBehaviour prefab, Vector3? pos = null, Transform parent = null)
		{
			if (!_prefabPool.ContainsKey(prefab)) { _prefabPool[prefab] = new GameObjectPool(prefab, parent); }

			return _prefabPool[prefab].Pop(pos);
		}

		public void Reset()
		{
			_prefabPool.Clear();
		}

        #endregion
    }
}