using UnityEngine;
using UnityEngine.UI;

namespace egads.system.scene
{
	public class SceneText : SceneWidget
	{
        #region Private Properties

        [SerializeField]
		private Text _frontText;
		[SerializeField]
		private Text _shadowText;

        #endregion

        #region Public Methods

        public void SetText(string content)
		{
			if (_frontText != null) { _frontText.text = content; }
			if (_shadowText != null) { _shadowText.text = content; }
		}

        #endregion
    }
}