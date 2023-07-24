﻿using UnityEngine;

namespace egads.tools.extensions
{
	public static class ArrayExtensions
	{
        #region Methods

        public static T PickRandom<T>(this T[] source)
		{
			if (source.Length == 0) { return default(T); }

			int i = Random.Range(0, source.Length);
			return source[i];
		}

		public static T First<T>(this T[] source)
		{
			if (source.Length == 0) { return default(T); }

			return source[0];
		}

		public static T Last<T>(this T[] source)
		{
			if (source.Length == 0) { return default(T); }

			return source[source.Length - 1];
		}

        #endregion
    }
}
