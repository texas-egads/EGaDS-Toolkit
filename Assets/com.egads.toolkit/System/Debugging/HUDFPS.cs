#if !UNITY_4_5

using UnityEngine;
using UnityEngine.UI;

namespace egads.system.debugging
{
    /// <summary>
    /// Attach this to a Text component to make a frames/second indicator.
    /// </summary>
    public class HUDFPS : MonoBehaviour
	{
        #region Public Properties

        public float updateInterval = 0.5F;

        #endregion

        #region Private Properties

        private float accum = 0; // FPS accumulated over the interval
		private int frames = 0; // Frames drawn over the interval
		private float timeleft;	// Left time for current interval

		private Text _textComponent;

        #endregion

        #region Unity Methods

        private void Start()
		{
			_textComponent = GetComponent<Text>();
			if (!_textComponent)
			{
				Debug.Log("UtilityFramesPerSecond needs a GUIText component!");
				enabled = false; // disable this component
				return;
			}
			timeleft = updateInterval;
		}

		private void Update()
		{
			timeleft -= Time.deltaTime;
			accum += Time.timeScale / Time.deltaTime;
			++frames;

			// Interval ended - update GUI text and start new interval
			if (timeleft <= 0.0)
			{
				// display two fractional digits (f2 format)
				float fps = accum / frames;
				string format = System.String.Format("{0:F2} FPS", fps);
				_textComponent.text = format;

				if (fps < 30) { _textComponent.color = new Color(1.0f, 0.35f, 0); }

				else
				{
					if (fps < 10) { _textComponent.color = Color.red; }
					else { _textComponent.color = Color.green; }	
				}

				timeleft = updateInterval;
				accum = 0.0F;
				frames = 0;
			}
		}

        #endregion
    }
}

#endif