using UnityEngine;

namespace egads.system.target
{
	public interface ITarget
	{
        #region Events

        event System.Action becomesInvalid;

        #endregion

        #region Properties

        Vector2 position2D { get; }

        #endregion
    }
}