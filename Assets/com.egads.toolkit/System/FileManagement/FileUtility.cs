using UnityEngine;

#if !UNITY_WEBPLAYER && !UNITY_METRO

using System.IO;

#endif

namespace egads.system.fileManagement
{
	public static class FileUtility
	{
        #region Public Properties

        public static bool canLoadAndSaveFiles
		{
			get
			{
				if (Application.platform == RuntimePlatform.WebGLPlayer) { return false; }
				return true;
			}
		}

        #endregion

        #region Non Web Build

#if !UNITY_WEBPLAYER && !UNITY_WINRT && !UNITY_WEBGL

        public static bool FileExists(string fileName) => File.Exists(fileName);

		public static void DeleteFile(string fileName)
		{
			File.Delete(fileName);
		}

		public static string ReadTextFile(string fileName) => File.ReadAllText(fileName);

		public static void WriteTextFile(string fileName, string content)
		{
			File.WriteAllText(fileName, content);
		}

#endif

        #endregion

        #region WebGL Build

#if UNITY_WEBGL

		public static bool FileExists(string fileName)
		{
			return false;  // Always return false
		}

		public static void DeleteFile(string fileName)
		{
			// Do nothing; will not be called
		}

		public static string ReadTextFile(string fileName)
		{
			return ""; // Do nothing
		}

		public static void WriteTextFile(string fileName, string content)
		{
			// Do nothing
		}

#endif

        #endregion

        #region WebPlayer Build

#if UNITY_WEBPLAYER

				public static bool FileExists(string fileName)
				{
					return false;  // Always return false
				}

				public static void DeleteFile(string fileName)
				{
					// Do nothing
				}

				public static string ReadTextFile(string fileName)
				{
					return ""; // Do nothing
				}

				public static void WriteTextFile(string fileName, string content)
				{
					// Do nothing
				}

#endif

        #endregion

        #region Metro Build

#if UNITY_METRO

				public static bool FileExists(string fileName) => UnityEngine.Windows.File.Exists(fileName);

				public static void DeleteFile(string fileName)
				{
					UnityEngine.Windows.File.Delete(fileName);
				}

				public static string ReadTextFile(string fileName)
				{
					var bytes = UnityEngine.Windows.File.ReadAllBytes(fileName);
					return System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length);
				}

				public static void WriteTextFile(string fileName, string content)
				{
					var bytes = System.Text.Encoding.UTF8.GetBytes(content);
					UnityEngine.Windows.File.WriteAllBytes(fileName, bytes);
				}

#endif

        #endregion

        #region WP8 Build

#if UNITY_WP8

		public static bool FileExists(string fileName) => UnityEngine.Windows.File.Exists(fileName);

		public static void DeleteFile(string fileName)
		{
			UnityEngine.Windows.File.Delete(fileName);
		}

		public static string ReadTextFile(string fileName)
		{
			using (StreamReader reader = new StreamReader(fileName, System.Text.Encoding.UTF8))
			{
				return reader.ReadToEnd();
			}
		}

		public static void WriteTextFile(string fileName, string content)
		{
			using (StreamWriter writer = new StreamWriter(fileName, false, System.Text.Encoding.UTF8))
			{
				writer.Write(content);
			}
		}

#endif

        #endregion
    }
}