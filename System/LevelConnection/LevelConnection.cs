using UnityEngine;

namespace egads.system.levelConnection
{
	public class LevelConnection : ScriptableObject
	{
        #region LevelName Properties

        public string levelName;
		public override string ToString() => levelName;

        #endregion
    }
}