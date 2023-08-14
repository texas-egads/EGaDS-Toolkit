using UnityEngine;

#if !UNITY_WEBPLAYER && !UNITY_METRO
using System.IO;
#endif

namespace egads.system.fileManagement
{
    public static class FileUtility
    {
        /// <summary>
        /// Indicates whether the platform can load and save files.
        /// </summary>
        public static bool canLoadAndSaveFiles
        {
            get
            {
                if (Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    return false;
                }
                return true;
            }
        }

        #region Non Web Build

#if !UNITY_WEBPLAYER && !UNITY_WINRT && !UNITY_WEBGL
        /// <summary>
        /// Checks if a file exists.
        /// </summary>
        public static bool FileExists(string fileName) => File.Exists(fileName);

        /// <summary>
        /// Deletes a file.
        /// </summary>
        public static void DeleteFile(string fileName)
        {
            File.Delete(fileName);
        }

        /// <summary>
        /// Reads the content of a text file.
        /// </summary>
        public static string ReadTextFile(string fileName) => File.ReadAllText(fileName);

        /// <summary>
        /// Writes content to a text file.
        /// </summary>
        public static void WriteTextFile(string fileName, string content)
        {
            File.WriteAllText(fileName, content);
        }
#endif

        #endregion

        #region WebGL Build

#if UNITY_WEBGL
        /// <summary>
        /// Checks if a file exists (WebGL platform).
        /// </summary>
        public static bool FileExists(string fileName)
        {
            return false; // Always return false for WebGL
        }

        /// <summary>
        /// Deletes a file (WebGL platform).
        /// </summary>
        public static void DeleteFile(string fileName)
        {
            // Do nothing; not supported in WebGL
        }

        /// <summary>
        /// Reads the content of a text file (WebGL platform).
        /// </summary>
        public static string ReadTextFile(string fileName)
        {
            return ""; // Do nothing; not supported in WebGL
        }

        /// <summary>
        /// Writes content to a text file (WebGL platform).
        /// </summary>
        public static void WriteTextFile(string fileName, string content)
        {
            // Do nothing; not supported in WebGL
        }
#endif

        #endregion
    }
}
