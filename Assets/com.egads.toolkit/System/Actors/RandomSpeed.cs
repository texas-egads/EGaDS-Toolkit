using UnityEngine;
using System;
using Random = UnityEngine.Random;

namespace egads.system.actors
{
	public class RandomSpeed : MonoBehaviour
	{
        #region Public Properties

        [Range(1f, 10f)]
		public float multiplier = 1f;

        #endregion

        #region Unity Methods

        private void Awake()
		{
			Actor2D actor = GetComponent<Actor2D>();
			actor.movementSpeed += Random.Range(1f, multiplier);
		}

        #endregion
    }
}