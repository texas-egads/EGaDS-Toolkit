using UnityEngine;
using System.Collections.Generic;
using System;
using egads.tools.json;

namespace egads.tools.text
{
    /// <summary>
    /// manages a text database for multiple languages
    /// </summary>
    public static class TextManager
    {
        #region Constants

        private const string DIRECTORY_IN_RESOURCES = "Text/";

        /*
         * Languages declared by Unity in SystemLanguage
         * 
         * How to add an additional language:
         * close project
         * add language name to the array of languages in languages.json in DIRECTORY_IN_RESOURCES
         * restart project, 'Window->Text Manager' should now show the updated languages
         * 
         * 
            Afrikaans
            Arabic
            Basque
            Belarusian
            Bulgarian
            Catalan
            Chinese
            Czech
            Danish
            Dutch
            English
            Estonian
            Faroese
            Finnish
            French
            German
            Greek
            Hebrew
            Icelandic
            Indonesian
            Italian
            Japanese
            Korean
            Latvian
            Lithuanian
            Norwegian
            Polish
            Portuguese
            Romanian
            Russian
            SerboCroatian
            Slovak
            Slovenian
            Spanish
            Swedish
            Thai
            Turkish
            Ukrainian
            Vietnamese
            Unknown
            Hungarian
         */

        #endregion

        #region Public Properties

        public static bool hasLoaded => _hasLoaded;
        public static bool contentWasModified => _contentWasModified;
        public static string standardLanguage
        {
            get
            {
                if (!_hasLoaded) { Load(); }
                return _standardLanguage;
            }
        }
        public static string currentLanguage
        {
            get
            {
                if (!_hasLoaded) { Load(); }
                return _currentLanguage;
            }
        }
        // Index of the currently selected language in the languages list
        // Gets calculated every time but should not be called often
        public static int currentLanguageIndex
        {
            get
            {
                if (!_hasLoaded) { Load(); }
                int index = 0;
                for (int i = 0; i < _languages.Count; i++)
                {
                    if (_languages[i] == _currentLanguage) { index = i; }
                }
                return index;
            }
        }
        public static string dataDirectory
        {
            get
            {
                string directoryName = DIRECTORY_IN_RESOURCES;
                if (directoryName[directoryName.Length - 1] != '/') { directoryName += '/'; }

                return directoryName;
            }
        }

        #endregion

        #region Private Properties

        private static bool _hasLoaded = false;
        private static bool _contentWasModified = false;

        // Data
        private static Dictionary<string, Dictionary<string, string>> _data = new Dictionary<string, Dictionary<string, string>>();

        // Languages
        private static List<string> _languages = new List<string>();

        // Standard language, which gets used when calling 'Text' if no entry for the current language was found
        private static string _standardLanguage = string.Empty;

        // Currently selected language, which gets used when calling 'Text'
        private static string _currentLanguage = string.Empty;

        // Event that gets called when the current language changes
        // Not called at first load as we assume that content gets initiated after TextManager has already loaded and initializes with the correct current language
        public static event Action languageChanged;

        #endregion

        #region Public Methods

        public static string Text(string key)
        {
            if (!_hasLoaded) { Load(); }
            return GetTextForLanguage(key, _currentLanguage);
        }

        public static string TextOrEmpty(string key)
        {
            if (!_hasLoaded) { Load(); }

            if (HasKey(key)) { return Text(key); }
            else { return ""; } 
        }

        public static string GetTextForLanguage(string key, string languageId)
        {
            if (!_hasLoaded) { Load(); }

            if (string.IsNullOrEmpty(languageId) || !_languages.Contains(languageId) || !_data.ContainsKey(languageId)) { return "<language " + languageId + " not found>"; }
            if (string.IsNullOrEmpty(key)) { return "<" + key + " is no valid key>"; }
            if (_data[languageId].ContainsKey(key)) { return _data[languageId][key]; }
            if (_data[_standardLanguage].ContainsKey(key)) { return _data[_standardLanguage][key]; }

            return "<text " + key + " not found>";
        }

        public static bool HasKey(string key)
        {
            if (!_hasLoaded) { Load(); }
            if (string.IsNullOrEmpty(key)) { return false; }

            return _data[_standardLanguage].ContainsKey(key);
        }

        public static List<string> GetLanguages()
        {
            if (!_hasLoaded) { Load(); }

            // Return copy of language list
            return new List<string>(_languages);
        }

        public static bool HasLanguage(string languageId)
        {
            if (!_hasLoaded) { Load(); }
            if (string.IsNullOrEmpty(languageId)) { return false; }

            return _languages.Contains(languageId);
        }

        public static List<string> Search(string searchString)
        {
            if (!_hasLoaded) { Load(); }

            List<string> result = new List<string>();

            Dictionary<string, string> standardData = _data[_standardLanguage];
            foreach (var item in standardData)
            {
                // Filter out keys which are reserved for the framework
                if (item.Key[0] == '#') { continue; }
                if (item.Key.ToLower().Contains(searchString.ToLower())) { result.Add(item.Key); }
            }

            return result;
        }

        public static void SetLanguage(string languageId)
        {
            if (!_hasLoaded) { Load(); }
            if (string.IsNullOrEmpty(languageId) || !_languages.Contains(languageId))
            {
                Debug.Log("TextManager: language '" + languageId + "' not found");
                return;
            }
            _currentLanguage = languageId;
            PlayerPrefs.SetString("LANGUAGE", _currentLanguage);
            if (languageChanged != null) { languageChanged(); }
        }

        #endregion

        #region Editor Methods

        public static void SetText(string key, string content, string languageId = "")
        {
            if (!_hasLoaded) { Load(); }

            if (languageId == "") { languageId = _standardLanguage; }
            if (string.IsNullOrEmpty(languageId) || !_languages.Contains(languageId)) { return; }   
            if (string.IsNullOrEmpty(key)) { return; }

            _data[languageId][key] = content;

            // Set field in standard language so it can be found by HasKey
            if (languageId != _standardLanguage && !_data[_standardLanguage].ContainsKey(key)) { _data[_standardLanguage][key] = ""; }

            MarkAsChanged();
        }

        // Copies an entry across all languages
        public static void Copy(string sourceKey, string destinationKey)
        {
            if (!_hasLoaded) { Load(); }
            if (string.IsNullOrEmpty(sourceKey) || string.IsNullOrEmpty(destinationKey)) { return; }

            foreach (var languageId in _languages)
            {
                if (_data[languageId].ContainsKey(sourceKey)) { _data[languageId][destinationKey] = _data[languageId][sourceKey]; }
            }
        }

        public static void SetStandardLanguage(string languageId)
        {
            if (!string.IsNullOrEmpty(languageId) && _languages.Contains(languageId)) { _standardLanguage = languageId; }

            MarkAsChanged();
        }

        public static void RemoveKey(string key)
        {
            if (string.IsNullOrEmpty(key)) { return; }
                

            for (int i = 0; i < _languages.Count; i++)
            {
                var languageData = _data[_languages[i]];
                if (languageData.ContainsKey(key)) { languageData.Remove(key); }
            }

            MarkAsChanged();
        }

        /// <summary>
        /// Deletes a language from the database
        /// </summary>
        public static void RemoveLanguage(string languageId)
        {
            if (string.IsNullOrEmpty(languageId) || _languages.Contains(languageId)) { return; }

            // Do not remove language if only one left
            if (_languages.Count == 1) { return; }

            // Remove from data
            _languages.Remove(languageId);
            _data.Remove(languageId);

            // Update standard IDs
            if (_currentLanguage == languageId) { _currentLanguage = _languages[0]; }
            if (_standardLanguage == languageId) { _standardLanguage = _languages[0]; }

            MarkAsChanged();
        }

        public static void InitNewDatabase()
        {
            Clear();

            AddLanguage(_standardLanguage);

            _hasLoaded = true;
            MarkAsChanged();
        }

        #endregion

        #region Loading and Saving

        public static void Load(TextAsset configAsset = null, List<TextAsset> languageFiles = null)
        {
            Clear();

            LoadConfigFile(configAsset);

            if (_languages.Count == 0) { InitNewDatabase(); }

			for (int i = 0; i < _languages.Count; i++)
			{
				if (languageFiles != null && i < languageFiles.Count) { LoadLanguageFile(_languages[i], languageFiles[i]); }
				else { LoadLanguageFile(_languages[i]); }
			}

            _hasLoaded = true;

            // Set system language as app language
            if (_languages.Contains(Application.systemLanguage.ToString())) { _currentLanguage = Application.systemLanguage.ToString(); }

            // Check if a player pref was set for language
            if (PlayerPrefs.HasKey("LANGUAGE"))
            {
                string languagePref = PlayerPrefs.GetString("LANGUAGE");
                if (_languages.Contains(languagePref)) { _currentLanguage = languagePref; }
            }
        }

        private static void Clear()
        {
            _data.Clear();
            _languages.Clear();

            _standardLanguage = "English";
            _currentLanguage = "English";
        }

        private static void AddLanguage(string languageId)
        {
            if (!string.IsNullOrEmpty(languageId) && !_languages.Contains(languageId))
            {
                _languages.Add(languageId);
                _data[languageId] = new Dictionary<string, string>();
            }
        }

		private static void LoadConfigFile(TextAsset asset = null)
        {
            // Load config from Resources folder
            if (asset == null) { asset = Resources.Load(dataDirectory + "languages") as TextAsset; }

            // Parse config file
            if (asset != null)
            {
                JSONObject data = JSONObject.Parse(asset.ToString());
                foreach (var item in data)
                {
                    if (item.Key == "languages")
                    {
                        var languageList = item.Value.Array;
                        foreach (var languageName in languageList) { AddLanguage(languageName.Str); }
                    }
                    else if (item.Key == "standard") { _standardLanguage = item.Value.Str; }
                }
            }
        }

        private static void LoadLanguageFile(string languageId, TextAsset asset = null)
        {
            // Load json file from Resources folder
			if (asset == null) { asset = Resources.Load(dataDirectory + "text_" + languageId) as TextAsset; }

            // Parse language data
            if (asset != null)
            {
                Dictionary<string, string> languageData = _data[languageId];

                JSONObject data = JSONObject.Parse(asset.ToString());
                foreach (var item in data) { languageData[item.Key] = item.Value.Str; }
            }
        }

        public static JSONObject GetConfigData()
        {
            JSONObject data = new JSONObject();

            data.Add("standard", _standardLanguage);

            JSONArray languageArray = new JSONArray();
            for (int i = 0; i < _languages.Count; i++) { languageArray.Add(new JSONValue(_languages[i])); }
            data.Add("languages", languageArray);

            return data;
        }

        public static JSONObject GetLanguageData(string languageId)
        {
            JSONObject data = new JSONObject();

            foreach (var item in _data[languageId]) { data.Add(item.Key, item.Value); }

            return data;
        }

        private static void MarkAsChanged()
        {
            _contentWasModified = true;
        }

        #endregion
    }
}
