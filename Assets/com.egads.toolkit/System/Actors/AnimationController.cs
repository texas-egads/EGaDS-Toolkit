using UnityEngine;
using System.Collections.Generic;
using egads.tools.utils;
using egads.system.timer;
using egads.system.gameManagement;

namespace egads.system.actors
{
	public class AnimationController : MonoBehaviour, IAnimationController
	{
        #region Animation State

        public enum AvatarAnimation
		{
			idle,
			walk,
			attack,
			die
		}

        #endregion

        #region Public Properties

        public GameObject displayObject;
		public List<SpriteRenderer> additionalSprites;

		public List<Renderer> excludeRendererFromEffects = new List<Renderer>();

        #endregion
		
        #region Private Properties

        private Actor2D _actor;
		private Animator _animator;

		private AvatarAnimation _currentAnimation = AvatarAnimation.idle;

		private FadingTimer _fadeInTimer = null;
		private FadingTimer _fadeOutTimer = null;

		private List<Material> _materials = new List<Material>();

		private bool _isInitialized = false;

        #endregion

        #region Unity Methods

        private void Awake()
		{
			_actor = GetComponent<Actor2D>();
			if (_actor != null) { _actor.stateChanged += ActorStateChangedHandler; }

			if (displayObject != null) { _animator = displayObject.GetComponentInChildren<Animator>(); }

			if (displayObject != null) { GatherRendererMaterials(); }
			else { Debug.Log("Actor " + gameObject.name + " has AnimationController but no display object assigned"); }

			_isInitialized = true;
		}

		private void Update()
		{
			// Show fadeout when game is running or has ended
			if (MainBase.Instance.state == GameState.Running || MainBase.Instance.state == GameState.Ended)
			{
				if (_fadeInTimer != null)
				{
					_fadeInTimer.Update();
					ApplyFadeIn();

					if (_fadeInTimer.hasEnded)
					{
						SetMaterialColor(Color.white);
						_fadeInTimer = null;
					}
				}
				else if (_fadeOutTimer != null)
				{
					_fadeOutTimer.Update();
					ApplyFadeOut();

					if (_fadeOutTimer.hasEnded)
					{
						SetMaterialColor(Color.white.WithAlpha(0));
						_fadeOutTimer = null;
					}
				}
			}
		}

        #endregion

        #region Public Methods

        public void SetVerticalLookDirection(float direction)
		{
			if (direction > 0) { _animator.SetFloat("lookY", 1f); }
			else if (direction < 0) { _animator.SetFloat("lookY", -1f); }
		}

		// Sets color for all sprite children
		public void SetMaterialColor(Color color)
		{
			for (int i = 0; i < _materials.Count; i++) { _materials[i].color = color; }

			for (int i = 0; i < additionalSprites.Count; i++) { additionalSprites[i].color = color; }
		}

		public void FadeIn(float time = 1f)
		{
			ClearFading();
			SetMaterialColor(Color.white.WithAlpha(0));
			_fadeInTimer = new FadingTimer(time, time);
			ApplyFadeIn();
		}

		public void FadeOut(float time = 1f)
		{
			ClearFading();
			_fadeOutTimer = new FadingTimer(0, time, time);
			ApplyFadeOut();
		}

		public void FadeOutAfterDeath()
		{
			if (_fadeOutTimer == null)
			{
				ClearFading();
				_fadeOutTimer = new FadingTimer(0, Actor2D.TIME_UNTIL_DESTRUCTION, 2.0f);
			}
		}

		public void Reset()
		{
			if (_isInitialized)
			{
				if (_animator != null) { SetAnimation(AvatarAnimation.idle); }

				_currentAnimation = AvatarAnimation.idle;
				_fadeOutTimer = null;
				SetMaterialColor(Color.white);
			}
		}

		public void SetAnimatorController(RuntimeAnimatorController controller)
		{
			_animator.runtimeAnimatorController = controller;
		}

		public void SetSpeed(float speed)
		{
			_animator.speed = speed;
		}

		public void ResetSpeed()
		{
			_animator.speed = 1f;
		}

        #endregion

        #region Private Methods

        private void ClearFading()
		{
			_fadeInTimer = null;
			_fadeOutTimer = null;
		}

		private void ActorStateChangedHandler(IActor activeActor, ActorState state)
		{
			if (state == ActorState.Dead) { SetAnimation(AvatarAnimation.die); }

			else if (state == ActorState.TakingAction) { SetAnimation(AvatarAnimation.attack); }

			else if (state == ActorState.Moving) { SetAnimation(AvatarAnimation.walk); }

			else if (state == ActorState.Idle) { SetAnimation(AvatarAnimation.idle); }
		}

		private void SetAnimation(AvatarAnimation anim)
		{
			if (_animator == null) { return; }

			if (_currentAnimation != anim)
			{
				switch (anim)
				{
                    // This is important
                    // Other parameters must be disabled to switch states, attack takes precedence over walk
                    case AvatarAnimation.idle:
						_animator.SetTrigger("idle");
						if (_animator.GetBool("walk")) { _animator.SetBool("walk", false); }
						break;

					case AvatarAnimation.walk:
						if (_animator.GetBool("idle")) { _animator.SetBool("idle", false); }
						if (_animator.GetBool("die")) { _animator.SetBool("die", false); }
						_animator.SetTrigger("walk");
						break;

					case AvatarAnimation.attack:
						if (_animator.GetBool("walk")) { _animator.SetBool("walk", false); }
						_animator.SetTrigger("attack");
						break;

					case AvatarAnimation.die:
						_animator.SetTrigger("die");

						// Set all other triggers to false so Mecanim knows exactly which state to trigger
						if (_animator.GetBool("walk")) { _animator.SetBool("walk", false); }
						if (_animator.GetBool("idle")) { _animator.SetBool("idle", false); }
						if (_animator.GetBool("attack")) { _animator.SetBool("attack", false); }
						break;

					default:
						_animator.SetTrigger(anim.ToString());
						break;
				}

				_currentAnimation = anim;
			}
		}

		private void ApplyFadeIn()
		{
			Color color = Color.white.WithAlpha(_fadeInTimer.progress);
            SetMaterialColor(color);
		}

		private void ApplyFadeOut()
		{
			Color color = Color.white.WithAlpha(_fadeOutTimer.progress);
			SetMaterialColor(color);
		}

		private void GatherRendererMaterials()
		{
			_materials.Clear();
			Renderer[] rendererList = displayObject.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < rendererList.Length; i++)
			{
				if (!excludeRendererFromEffects.Contains(rendererList[i])) { _materials.Add(rendererList[i].material); }
			}
		}

        #endregion
    }
}