using UnityEngine;

namespace egads.tools.extensions
{
    public static class StringExtensions
    {
        #region Methods

        public static string Color(this string s, Color c)
        {
            string cHex = ColorUtility.ToHtmlStringRGBA(c);
            return "<color=#" + cHex + ">" + s + "</color>";
        }

        public static string Bold(this string s) => "<b>" + s + "</b>";

        public static string Italic(this string s) => "<i>" + s + "</i>";

        #endregion
    }
}

