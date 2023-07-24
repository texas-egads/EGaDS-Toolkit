using UnityEngine;

namespace egads.system.pathFinding
{
	public interface IPathField
	{
        #region Methods

        void GetPath(Vector2 startPos, Vector2 endPos, Vector2Path vectorPath);
		void UpdateField(Bounds bounds);

		#endregion
	}
}