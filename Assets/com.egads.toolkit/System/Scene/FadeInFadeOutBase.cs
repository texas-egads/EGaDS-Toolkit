using UnityEngine;
using egads.system.stateMachine;

namespace egads.system.scene
{
	public abstract class FadeInFadeOutBase : MonoBehaviour
	{
        #region Public Properties

        public float fadeDuration = 0.5f;
		public bool isShowing => (state == ShowAndHideState.Shown || state == ShowAndHideState.IsShowing);
		public bool isVisible => (state != ShowAndHideState.Hidden);
		public float amplitude
		{
			get
			{
				if (state == ShowAndHideState.Shown) { return 1.0f; }
				else if (state == ShowAndHideState.Hidden) { return 0; }
				else { return fadeTime / fadeDuration; }
			}
		}

        #endregion

        #region Private Properties

        [SerializeField]
		private ShowAndHideState state = ShowAndHideState.Shown;
		private float fadeTime;

        #endregion

        #region Unity Methods

        protected virtual void Awake()
		{
			fadeTime = fadeDuration;

			if (state == ShowAndHideState.Hidden)
			{
				EnableComponents(false);
				fadeTime = 0;
			}
		}

		private void Update()
		{
			if (state == ShowAndHideState.IsHiding)
			{
				fadeTime -= Time.deltaTime;

				if (fadeTime <= 0)
				{
					fadeTime = 0;
					state = ShowAndHideState.Hidden;

					ApplyAmplitude(0);
					EnableComponents(false);
				}
				else { ApplyAmplitude(amplitude); }
			}
			else if (state == ShowAndHideState.IsShowing)
			{
				fadeTime += Time.deltaTime;
				if (fadeTime >= fadeDuration)
				{
					fadeTime = fadeDuration;
					state = ShowAndHideState.Shown;

					ApplyAmplitude(1.0f);
				}
				else { ApplyAmplitude(amplitude); }
			}
		}

        #endregion

        #region Public Methods

        public void Show()
		{
			if (state == ShowAndHideState.Hidden)
			{
				fadeTime = 0;
				state = ShowAndHideState.IsShowing;

				EnableComponents(true);
				ApplyAmplitude(0);
			}
			else if (state == ShowAndHideState.IsHiding) { state = ShowAndHideState.IsShowing; }
		}

		public void Hide()
		{
			if (state == ShowAndHideState.Shown)
			{
				fadeTime = fadeDuration;
				state = ShowAndHideState.IsHiding;
			}
			else if (state == ShowAndHideState.IsShowing) { state = ShowAndHideState.IsHiding; }
		}

		public void Toggle()
		{
			if (isShowing) { Hide(); }
			else { Show(); }
		}

        #endregion

        #region Private Methods

        protected virtual void EnableComponents(bool show)
		{
			gameObject.SetActive(show);
		}

		protected abstract void ApplyAmplitude(float amplitude);

        #endregion
    }
}