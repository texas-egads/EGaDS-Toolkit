using UnityEngine;
using egads.tools.utils;

namespace egads.system.UI
{
	public class SceneFader
	{
        #region Fade Type

        public enum FadeType
		{
			none,
			FadeOutFinished,
			fadeIn,
			fadeOutFailure,
			fadeOutSuccess
		}

        #endregion

        #region Constants

        const float standardFadeDuration = 1.0f;

        #endregion

        #region Delegates and Events

        public delegate void ChangeFadeTypeDelegate(FadeType type);
		public event ChangeFadeTypeDelegate stateChanged;

        #endregion

        #region Private Properties

        private Texture2D _whiteTexture;

		public Texture2D whiteTexture => _whiteTexture;

		private Rect _screenRect;
		private Color _currentColor;
		private Color _startColor;
		private Color _targetColor;
		private float _fadeTime = 0.6f;
		private float _currentTime = 0;

		private FadeType _fadeState = FadeType.none;
		private FadeType fadeState
		{
			get
			{
				return _fadeState;
			}
			set
			{
				if (value != _fadeState)
				{
					_fadeState = value;

					if (stateChanged != null) { stateChanged(value); }
				}
			}
		}

        #endregion

        #region Color Properties

        private Color _guiColor = new Color(0f, 0f, 0f, 1f);
		public Color guiColor
		{
			set
			{
				_guiColor = value;
			}
		}

		public Color failureColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        #endregion

        #region Constructor

        public SceneFader()
		{
			// Create white blank texture
			_whiteTexture = Utilities.CreateBlankTexture(Color.white);

			_currentColor = Color.white;
		}

        #endregion

        #region Public Methods

        public void Update()
		{
			if (fadeState != FadeType.none && fadeState != FadeType.FadeOutFinished) { _currentTime += Time.deltaTime; }

			switch (fadeState)
			{
				case FadeType.none:
					break;

				case FadeType.fadeIn:
					FadeInUpdate();
					break;

				case FadeType.fadeOutFailure:
					FadeOutUpdate();
					break;

				case FadeType.fadeOutSuccess:
					FadeOutUpdate();
					break;

				case FadeType.FadeOutFinished:
					FadeOutUpdate();
					break;

				default:
					break;
			}
		}

		public void OnGUI()
		{
			if (fadeState != FadeType.none)
			{
				GUI.color = _currentColor;
				GUI.DrawTexture(_screenRect, _whiteTexture, ScaleMode.StretchToFill);
			}
		}

		public void Clear()
		{
			fadeState = FadeType.none;
		}

		public void FadeIn(float? fadeTime = null)
		{
			CalculateScreenRect();

			fadeState = FadeType.fadeIn;
			_startColor = _guiColor;
			_targetColor = _guiColor;
			_targetColor.a = 0f;
			_currentTime = 0;

			_currentColor = _startColor;

			_fadeTime = fadeTime.GetValueOrDefault(standardFadeDuration);
		}

		public void FadeOutFailure(float? fadeTime = null)
		{
			CalculateScreenRect();

			fadeState = FadeType.fadeOutFailure;
			_startColor = failureColor;
			_startColor.a = 0f;
			_targetColor = failureColor;
			_currentTime = 0;

			_currentColor = _startColor;

			_fadeTime = fadeTime.GetValueOrDefault(standardFadeDuration);
		}

		public void FadeOutSuccess(float? fadeTime = null)
		{
			CalculateScreenRect();

			fadeState = FadeType.fadeOutSuccess;
			_startColor = _guiColor;
			_startColor.a = 0f;
			_targetColor = _guiColor;
			_currentTime = 0;

			_currentColor = _startColor;

			_fadeTime = fadeTime.GetValueOrDefault(standardFadeDuration);
		}

        #endregion

        #region Private Methods

        void CalculateScreenRect()
		{
			_screenRect = new Rect(0, 0, Screen.width, Screen.height);
		}

		void FadeInUpdate()
		{
			float progress = _currentTime / _fadeTime;
			_currentColor = Color.Lerp(_startColor, _targetColor, progress);

			if (progress >= 1) { fadeState = FadeType.none; }
		}

		void FadeOutUpdate()
		{
			float progress = _currentTime / _fadeTime;
			if (progress >= 1f) { progress = 1f; }
			_currentColor = Color.Lerp(_startColor, _targetColor, progress);

			if (progress >= 1) { fadeState = FadeType.FadeOutFinished; }
		}

        #endregion
    }
}