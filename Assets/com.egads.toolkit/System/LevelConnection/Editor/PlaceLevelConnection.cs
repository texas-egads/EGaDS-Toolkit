using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using egads.system.levelConnection;

namespace egads.code
{
    public class PlaceLevelConnection
    {
        [MenuItem("EGaDS-Toolkit/Level Connection Entry #F3")]
        public static void PlaceLevelConnectionMenuEntry()
        {
            CreateConnection();
        }

        [MenuItem("EGaDS-Toolkit/Level Connection Entry #F3", true)]
        public static bool PlaceLevelConnectionMenuEntryValidator()
        {
            return !Application.isPlaying;
        }

        private static void CreateConnection()
        {
            Transform active = Selection.activeTransform;

            if (active == null)
            {
                Debug.LogWarning("No GameObject selected");
                return;
            }

            LevelEntry entry = active.gameObject.AddComponent<LevelEntry>();

            //LevelConnection connection = new LevelConnection();
            LevelConnection connection = ScriptableObject.CreateInstance<LevelConnection>();
            connection.levelName = Path.GetFileNameWithoutExtension(EditorSceneManager.GetActiveScene().path);

            string fileName = EditorUtility.SaveFilePanelInProject("Save Connection", "Connection", "asset", "Save new Connection");

            if (!string.IsNullOrEmpty(fileName))
            {
                string shortName = Path.GetFileNameWithoutExtension(fileName);

                AssetDatabase.CreateAsset(connection, fileName);
                AssetDatabase.SaveAssets();

                entry.incomingConnection = connection;

                active.name = shortName;
            }
        }
    }
}
