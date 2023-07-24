using UnityEngine;
using System.Collections.Generic;
using System;
using System.Text;

namespace egads.tools.utils
{
	public static class Utilities
	{
        #region Properties

        public static Vector2 mouseInScreenSpace => new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
        public static bool mouseIsAtScreenEdgeOrBeyond => Input.mousePosition.x <= 0 || Input.mousePosition.y <= 0 || Input.mousePosition.x >= Screen.width || Input.mousePosition.y >= Screen.height;
        public static Vector2 relativeMousePosition => new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
        public static Vector2 mouseInWorldSpace => Camera.main.ScreenToWorldPoint(Input.mousePosition);
        public static Rect screenRect => new Rect(0, 0, Screen.width, Screen.height);

        #endregion

        #region Methods

        public static Texture2D CreateBlankTexture(Color color, TextureFormat format = TextureFormat.RGB24, int size = 2)
		{
			// Create empty texture
			Texture2D texture = new Texture2D(size, size, format, false, true);

			// Get all pixels as an array
			var cols = texture.GetPixels();
			for (int i = 0; i < cols.Length; i++) { cols[i] = color; }

			// Important steps to save changed pixel values
			texture.SetPixels(cols);
			texture.Apply();

			texture.hideFlags = HideFlags.HideAndDontSave;

			return texture;
		}

		public static Vector2 GetMouseInWorldSpace(Camera camera) => camera.ScreenToWorldPoint(Input.mousePosition);

		public static Vector3 GetNormalizedDirection(Vector3 fromPos, Vector3 toPos)
		{
			Vector3 direction = toPos - fromPos;
			return direction.normalized;
		}

		public static Vector3 GetPositionByObjectName(string objectName)
		{
			GameObject obj = GameObject.Find(objectName);
			if (obj != null) { return obj.transform.position; }
			else
			{
				Debug.LogWarning("Position from Object " + objectName + " not found");
				return Vector3.zero;
			}
		}

		public static List<string> ConvertToStrings<T>(List<T> originalList)
		{
			List<string> newList = new List<string>();
			for (int i = 0; i < originalList.Count; i++) { newList.Add(originalList[i].ToString()); }

			return newList;
		}

		public static void DestroyInScene<T>(bool immediately = false) where T : MonoBehaviour
		{
			var objects = GameObject.FindObjectsOfType<T>();
			for (int i = 0; i < objects.Length; i++)
			{
				if (immediately) { GameObject.DestroyImmediate(objects[i].gameObject); }
				else { GameObject.Destroy(objects[i].gameObject); }
			}
		}

		public static float TestMethod(Action actionToBeTested, string testName, int numberOfExecutions = 50, bool printDebug = true)
		{
			float startTime = Time.realtimeSinceStartup;

			for (int i = 0; i < numberOfExecutions; i++) { actionToBeTested(); }

			float averageTime = (Time.realtimeSinceStartup - startTime) / (float)numberOfExecutions;
			if (printDebug) { Debug.Log("Testing " + testName + ": " + averageTime.ToString("F5") + " ms in average"); }

			return averageTime;
		}
		public static Color ColorWithAlpha(Color color, float alpha) => new Color(color.r, color.g, color.b, alpha);

		public static Color ColorFromInt(int red, int green, int blue) => new Color(red / 255f, green / 255f, blue / 255f);

		public static Color WithAlpha(this Color source, float alpha)
		{
			source.a = alpha;
			return source;
		}

		public static string ColorToHex(Color32 color)
		{
			string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2") + color.a.ToString("X2");
			return hex;
		}

		public static Color HexToColor(string hex)
		{
			byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
			byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
			byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
			byte a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
			return new Color32(r, g, b, a);
		}

		public static string ColorizeString(string desc, Color color) => "<color=#" + ColorToHex(color) + ">" + desc + "</color>";

		public static string InsertSpaceBetweenUppercase(string source)
		{
			StringBuilder builder = new StringBuilder();
			foreach (char c in source)
			{
				if (Char.IsUpper(c) && builder.Length > 0) { builder.Append(' '); }
				builder.Append(c);
			}
			return builder.ToString();
		}

		public static Vector2 RoundVector(Vector2 vec)
		{
			vec.x = Mathf.Round(vec.x);
			vec.y = Mathf.Round(vec.y);

			return vec;
		}

		public static Vector3 RoundVector(Vector3 vec)
		{
			vec.x = Mathf.Round(vec.x);
			vec.y = Mathf.Round(vec.y);
			vec.z = Mathf.Round(vec.z);

			return vec;
		}

        #endregion

        #region Layer Mask Methods

        // Calculates a layermask by combining all named layers
        public static int LayerMask(params string[] layerNames)
        {
            // No layers selected
            int mask = 0;  

            for (int i = 0; i < layerNames.Length; i++)
            {
                int newMask = 1 << UnityEngine.LayerMask.NameToLayer(layerNames[i]);
                mask = mask | newMask;
            }

            return mask;
        }

        // Calculates a layermask by adding layerNames to default layer mask
        public static int LayerMaskIncludingDefault(params string[] layerNames)
        {
            // LayerMask for 'Default' Layer
            int mask = 1;  

            for (int i = 0; i < layerNames.Length; i++)
            {
                int newMask = 1 << UnityEngine.LayerMask.NameToLayer(layerNames[i]);
                mask = mask | newMask;
            }

            return mask;
        }

        // Calculates a layermask by subtracting all named layers
        public static int LayerMaskAllExcluding(params string[] layerNames)
        {
            // All layers selected
            int mask = int.MaxValue; 

            for (int i = 0; i < layerNames.Length; i++)
            {
                int newMask = 1 << UnityEngine.LayerMask.NameToLayer(layerNames[i]);

                // we 'subtract' the bits that are in both masks
                mask = mask ^ newMask;   // 0xff00 ^ 0x0ff0 => 0xf0f0;
            }

            return mask;
        }

        #endregion
    }
}