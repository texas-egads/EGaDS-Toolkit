using System;

namespace egads.tools.utils
{
    public static class EnumUtils
    {
        #region Methods

        public static T ParseEnum<T>(string value, T defaultValue) where T : struct, IConvertible
        {
            if (!(typeof(T).IsEnum)) { throw new ArgumentException("T must be an enumerated type"); }

            if (string.IsNullOrEmpty(value)) { return defaultValue; }

            try { return (T)Enum.Parse(typeof(T), value, true); }
            catch { return defaultValue; }
        }

        #endregion
    }
}
