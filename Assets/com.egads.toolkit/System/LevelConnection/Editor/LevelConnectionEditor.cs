using UnityEditor;

namespace egads.system.levelConnection
{
	[CustomEditor(typeof(LevelConnection))]
	public class LevelConnectionEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
		}
	}
}