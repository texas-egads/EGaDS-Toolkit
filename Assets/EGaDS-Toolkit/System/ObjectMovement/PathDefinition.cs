using UnityEngine;
using System.Collections.Generic;

namespace egads.system.objectMovement
{
	public class PathDefinition : MonoBehaviour
	{
        #region Public Properties

        public Transform[] points;

        #endregion

        #region Path Methods

        public IEnumerator<Transform> GetPathEnumerator()
		{
			if (points == null || points.Length < 1) { yield break; }

			var direction = 1;
			var index = 0;

			while (true)
			{
				yield return points[index];

				if (points.Length == 1) { continue; }

				if (index <= 0) { direction = 1; }
				else if (index >= points.Length - 1) { direction = -1; }

				index += direction;
			}
		}

        #endregion

        #region Unity Methods

        public void OnDrawGizmos()
		{
			if (points == null || points.Length <= 1) { return; }

			for (int i = 1; i < points.Length; i++) { Gizmos.DrawLine(points[i - 1].position, points[i].position); }
		}

        #endregion
    }
}