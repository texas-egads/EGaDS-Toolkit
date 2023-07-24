﻿using UnityEngine;
using System.Collections.Generic;

namespace egads.tools.objects
{
    public class GameObjectPool
    {
        #region Private Properties

        private MonoBehaviour _prefab;

		private Transform _parent = null;

        private Stack<IPooledObject> _pool = new Stack<IPooledObject>();
        private List<IPooledObject> _activeObjects = new List<IPooledObject>();

        #endregion

        #region Constructor

        public GameObjectPool(MonoBehaviour prefab, Transform parent = null)
		{
			_prefab = prefab;
			_parent = parent;
		}

        #endregion

        #region Public Methods

        // Get an Object from the pool
        public IPooledObject Pop(Vector3? pos = null)
        {
            IPooledObject newObject;

            if (_pool.Count == 0)
            {
                newObject = CreateNewObject(pos);

                if (newObject == null) { return default(IPooledObject); }
                newObject.isUsedByObjectPool = true;
            }
            else
            {
                newObject = _pool.Pop();
                newObject.ToggleOn();

                // Set position
                if (newObject is MonoBehaviour && pos != null) { (newObject as MonoBehaviour).transform.position = pos.Value; }
            }
            _activeObjects.Add(newObject);
            newObject.getsDisabled += ObjectGetsDisabled;

            return newObject;
        }

        // Return an Object to the pool
        public void Push(IPooledObject item)
        {
            item.ToggleOff();
            item.getsDisabled -= ObjectGetsDisabled;

            if (_activeObjects.Contains(item)) { _activeObjects.Remove(item); }
            _pool.Push(item);
        }

		public void Clear()
		{
			_activeObjects.Clear();
			_pool.Clear();
		}

        #endregion

        #region Private Methods

        private IPooledObject CreateNewObject(Vector3? pos = null)
        {
			if (_prefab == null)
			{
				Debug.LogError("ObjectPool has no prefab");
				return null;
			}

			MonoBehaviour newObject = GameObjectFactory.Instantiate(_prefab, position: pos);

			if (_parent != null) { newObject.transform.SetParent(_parent); }

			IPooledObject instance = newObject as IPooledObject;
            if (instance == null) { Debug.LogError("Object " + _prefab + " is not of type " + typeof(IPooledObject)); }

            return instance;
        }

        private void ObjectGetsDisabled(IPooledObject obj)
        {
            Push(obj);
        }

        #endregion
    }

}