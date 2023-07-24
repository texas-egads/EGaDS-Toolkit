using UnityEngine.UI;
using egads.tools.utils;

namespace egads.system.scene
{
	public class FadeInFadeOutText : FadeInFadeOutBase
	{
        #region Private Properties

        private Text[] _textDisplays;

        #endregion

        #region Unity Methods

        protected override void Awake()
		{
			_textDisplays = GetComponentsInChildren<Text>();

			base.Awake();
		}

        #endregion

        #region Private Methods

        protected override void ApplyAmplitude(float amplitude)
		{
			for (int i = 0; i < _textDisplays.Length; i++) { _textDisplays[i].color = Utilities.ColorWithAlpha(_textDisplays[i].color, amplitude); }
		}

        #endregion
    }
}