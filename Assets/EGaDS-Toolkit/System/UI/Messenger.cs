﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using egads.system.timer;

namespace egads.system.UI
{
	public class Messenger : MonoBehaviour
	{
        #region Public Properties

        public float displayDuration = 2f;

        #endregion

        #region Private Properties

        private FadingTimer _timer;

		[SerializeField]
		private Text[] _text = null;

		private string _currentMessage = "";

        #endregion

        #region Unity Methods

        public void Awake()
		{
			_timer = new FadingTimer(0.4f, displayDuration, 0.4f);
			_timer.Stop();

			if (_text == null || _text.Length == 0) { _text = GetComponentsInChildren<Text>(); }

			if (_text == null || _text.Length == 0)
			{
				// The MessengerText Object will be inside the MainCanvas
				var messengerObject = GameObject.Find("MessengerText") as GameObject;
				if (messengerObject != null) { _text = messengerObject.GetComponentsInChildren<Text>(); }
			}

			if (_text != null)
			{
				for (int i = 0; i < _text.Length; i++) { _text[i].enabled = false; }
			}
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		public void Update()
		{
			if (!_timer.hasEnded)
			{
				_timer.Update();
				UpdateColor();
			}

			if (_timer.hasEnded)
			{
				for (int i = 0; i < _text.Length; i++) { _text[i].enabled = false; }
			}
		}

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
			_timer.Stop();
			UpdateColor();
		}

		public void SetText(string newMessage)
		{
			for (int i = 0; i < _text.Length; i++) { _text[i].text = newMessage; }
		}

        #endregion

        #region Public Methods

        public void Message(string newMessage)
		{
			Message(newMessage, displayDuration);
        }

		public void Message(string newMessage, float duration)
		{
			for (int i = 0; i < _text.Length; i++)
			{
				_text[i].text = newMessage;
				_text[i].enabled = true;
			}

			_timer.SetDuration(duration);
			
			// if message is the same, let it be or reset timer at point of completed fade in
			if (!string.IsNullOrEmpty(newMessage) && newMessage == _currentMessage)
			{
				if (!_timer.IsInFadeIn) { _timer.SetToFadedInPoint(); }			
			}
			else { _timer.Reset(); }

			UpdateColor();
			_currentMessage = newMessage;
		}

        #endregion

        #region Private Methods

        private void UpdateColor()
		{
			float alpha = _timer.progress;
			if (_timer.hasEnded) { alpha = 0; }

			Color displayColor = new Color(1.0f, 1.0f, 1.0f, _timer.progress);
			if (_timer.hasEnded) { displayColor.a = 0; }

			for (int i = 0; i < _text.Length; i++)
			{
				Color color = _text[i].color;
				color.a = alpha;
				_text[i].color = color;
            }
		}

        #endregion
    }
}