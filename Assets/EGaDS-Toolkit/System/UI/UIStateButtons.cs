﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using egads.system.gameManagement;
using egads.system.input;

namespace egads.system.UI
{
	public class UIStateButtons : MonoBehaviour, IUIState<GameState>
	{
        #region Public Properties

        public GameState gameState => _gameState;

        #endregion

        #region Private Properties

        [SerializeField]
		private GameState _gameState = GameState.None;

		protected int _selected = -1;

		protected List<Button> _allButtons = new List<Button>();
		protected List<Button> _buttons = new List<Button>();

        #endregion

        #region Unity Methods

        public virtual void Awake()
		{
			var layoutContainerObject = GetComponentInChildren<VerticalLayoutGroup>();
			if (layoutContainerObject == null)
			{
				Debug.LogWarning("UIButtonsState has no VerticalLayoutGroup");
				return;
			}

			var layoutContainer = layoutContainerObject.transform;
			int childCount = layoutContainer.childCount;

			for (int i = 0; i < childCount; i++)
			{
				Transform child = layoutContainer.GetChild(i);
				Button button = child.GetComponentInChildren<Button>();
				if (button != null)
				{
					_allButtons.Add(button);
					_buttons.Add(button);
				}
			}

			foreach (var button in _allButtons) { button.onClick.AddListener(ButtonClick); }
		}

        #endregion

        #region IUIState Implementation

        public virtual void Enter()
		{
			if (MainBase.Instance != null && MainBase.Instance.baseNavigationInput != null)
				MainBase.Instance.baseNavigationInput.Add(this);

			if (MainBase.Instance != null && MainBase.Instance.inputController != null && (MainBase.Instance.inputController.inputScheme == InputScheme.Keyboard || MainBase.Instance.inputController.inputScheme == InputScheme.Gamepad) && _buttons.Count > 0)
			{
				_selected = 0;
				_buttons[_selected].Select();
			}
			else { _selected = -1; }
		}

		public virtual void Exit()
		{
			if (MainBase.Instance != null && MainBase.Instance.baseNavigationInput != null) { MainBase.Instance.baseNavigationInput.Remove(this); }

			EventSystem.current.SetSelectedGameObject(null, null);
		}

		public virtual void SetActive()
		{
			gameObject.SetActive(true);
		}

		public virtual void SetInactive()
		{
			gameObject.SetActive(false);
		}

        #endregion

        #region INavigationInput Implementation

        public virtual void InputUp()
		{
			_selected--;

			if (_selected < 0) { _selected = _buttons.Count - 1; }

			_buttons[_selected].Select();
		}

		public virtual void InputDown()
		{
			_selected++;

			if (_selected >= _buttons.Count) { _selected = 0; }

			_buttons[_selected].Select();
		}

		public virtual void InputLeft()
		{

		}

		public virtual void InputRight()
		{

		}

		public virtual void InputEnter()
		{
			if (_selected >= 0) { _buttons[_selected].onClick.Invoke(); }
		}

		public virtual void InputBack()
		{

		}

		public bool acceptsSecondaryButtons => true;

        #endregion

        #region Utilities

        private void ButtonClick()
		{
			MainBase.BaseAudioManager.PlayButtonSound();
		}

		protected void EnableAllButtons()
		{
			_buttons.Clear();
			foreach (var item in _allButtons)
			{
				item.gameObject.SetActive(true);
				_buttons.Add(item);
			}
		}

		protected void DisableButton(int index)
		{
			if (index < 0 || index >= _allButtons.Count) { return; }

			_allButtons[index].gameObject.SetActive(false);

			if (_buttons.Contains(_allButtons[index])) { _buttons.Remove(_allButtons[index]); }
		}

        #endregion
    }
}