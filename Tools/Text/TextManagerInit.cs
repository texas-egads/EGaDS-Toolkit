using UnityEngine;
using System.Collections.Generic;

namespace egads.tools.text
{
	public class TextManagerInit : MonoBehaviour
	{
        #region Public Properties

        public TextAsset languages;
		public List<TextAsset> languageFiles;

        #endregion

        #region Methods

        public void Init()
		{
			TextManager.Load(languages, languageFiles);
		}

        #endregion
    }
}