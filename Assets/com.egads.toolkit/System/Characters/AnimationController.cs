using UnityEngine;
using System.Collections.Generic;
using egads.tools.utils;
using egads.system.timer;
using egads.system.gameManagement;

namespace egads.system.characters
{
    /// <summary>
    /// Controls the animations of the character using the Unity Animator system.
    /// </summary>
    public class AnimationController : MonoBehaviour, IAnimationController
    {
        #region Animation State

        /// <summary>
        /// Enumeration representing different avatar animations.
        /// </summary>
        public enum AvatarAnimation
        {
            /// <summary>
            /// The idle animation state of the avatar.
            /// </summary>
            idle,

            /// <summary>
            /// The walk animation state of the avatar.
            /// </summary>
            walk,

            /// <summary>
            /// The attack animation state of the avatar.
            /// </summary>
            attack,

            /// <summary>
            /// The die animation state of the avatar.
            /// </summary>
            die
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The GameObject that holds the Animator component for the animations.
        /// </summary>
        public GameObject displayObject;

        /// <summary>
        /// Additional SpriteRenderers to change color when fading.
        /// </summary>
        public List<SpriteRenderer> additionalSprites;

        /// <summary>
        /// Renderers to exclude from color effects.
        /// </summary>
        public List<Renderer> excludeRendererFromEffects = new List<Renderer>();

        #endregion

        #region Private Properties

        // Reference to the Character2D component attached to this GameObject.
        private Character2D _character;

        // Reference to the Animator component for playing animations.
        private Animator _animator;

        // Current avatar animation.
        private AvatarAnimation _currentAnimation = AvatarAnimation.idle;

        // Timer for fade-in  and fade-out effects.
        private FadingTimer _fadeInTimer = null;  
        private FadingTimer _fadeOutTimer = null;

        // List of materials of the renderer components.
        private List<Material> _materials = new List<Material>();

        // Flag indicating if the AnimationController is initialized.
        private bool _isInitialized = false; 

        #endregion

        #region Unity Methods

        private void Awake()
        {
            // Get the Character2D component attached to this GameObject.
            _character = GetComponent<Character2D>(); 
            if (_character != null) { _character.stateChanged += CharacterStateChangedHandler; }

            if (displayObject != null) { _animator = displayObject.GetComponentInChildren<Animator>(); }

            // Gather the materials of renderers for color effects.
            if (displayObject != null) { GatherRendererMaterials(); } 
            else { Debug.Log("Character " + gameObject.name + " has AnimationController but no display object assigned"); }

            _isInitialized = true;
        }

        private void Update()
        {
            // Show fadeout when the game is running or has ended
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

        /// <summary>
        /// Sets the vertical look direction of the avatar animation.
        /// </summary>
        /// <param name="direction">The vertical look direction (positive or negative).</param>
        public void SetVerticalLookDirection(float direction)
        {
            if (direction > 0) { _animator.SetFloat("lookY", 1f); }
            else if (direction < 0) { _animator.SetFloat("lookY", -1f); }
        }

        /// <summary>
        /// Sets the color of all sprite children.
        /// </summary>
        /// <param name="color">The color to set for the sprites.</param>
        public void SetMaterialColor(Color color)
        {
            for (int i = 0; i < _materials.Count; i++) { _materials[i].color = color; }

            for (int i = 0; i < additionalSprites.Count; i++) { additionalSprites[i].color = color; }
        }

        /// <summary>
        /// Initiates a fade-in effect for the avatar animation.
        /// </summary>
        /// <param name="time">The duration of the fade-in effect.</param>
        public void FadeIn(float time = 1f)
        {
            ClearFading();
            SetMaterialColor(Color.white.WithAlpha(0));
            _fadeInTimer = new FadingTimer(time, time);
            ApplyFadeIn();
        }

        /// <summary>
        /// Initiates a fade-out effect for the avatar animation.
        /// </summary>
        /// <param name="time">The duration of the fade-out effect.</param>
        public void FadeOut(float time = 1f)
        {
            ClearFading();
            _fadeOutTimer = new FadingTimer(0, time, time);
            ApplyFadeOut();
        }

        /// <summary>
        /// Initiates a fade-out effect after the avatar's death.
        /// </summary>
        public void FadeOutAfterDeath()
        {
            if (_fadeOutTimer == null)
            {
                ClearFading();
                _fadeOutTimer = new FadingTimer(0, Character2D.TIME_UNTIL_DESTRUCTION, 2.0f);
            }
        }

        /// <summary>
        /// Resets the animation controller to the default state.
        /// </summary>
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

        /// <summary>
        /// Sets the animator controller for the avatar animation.
        /// </summary>
        /// <param name="controller">The new animator controller.</param>
        public void SetAnimatorController(RuntimeAnimatorController controller)
        {
            _animator.runtimeAnimatorController = controller;
        }

        /// <summary>
        /// Sets the playback speed of the avatar animation.
        /// </summary>
        /// <param name="speed">The playback speed value (1 is normal speed).</param>
        public void SetSpeed(float speed)
        {
            _animator.speed = speed;
        }

        /// <summary>
        /// Resets the playback speed of the avatar animation to normal (1).
        /// </summary>
        public void ResetSpeed()
        {
            _animator.speed = 1f;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Clears the fading timers, resetting the fade-in and fade-out effects.
        /// </summary>
        private void ClearFading()
        {
            _fadeInTimer = null;
            _fadeOutTimer = null;
        }

        /// <summary>
        /// Handles the change in the character's state and updates the avatar animation accordingly.
        /// </summary>
        /// <param name="activeCharacter">The active character implementing the ICharacter interface.</param>
        /// <param name="state">The new state of the character.</param>
        private void CharacterStateChangedHandler(ICharacter activeCharacter, CharacterState state)
        {
            if (state == CharacterState.Dead) { SetAnimation(AvatarAnimation.die); }
            else if (state == CharacterState.TakingOrder) { SetAnimation(AvatarAnimation.attack); }
            else if (state == CharacterState.Moving) { SetAnimation(AvatarAnimation.walk); }
            else if (state == CharacterState.Idle) { SetAnimation(AvatarAnimation.idle); }
        }

        /// <summary>
        /// Sets the avatar animation based on the specified animation enum.
        /// </summary>
        /// <param name="anim">The animation to set.</param>
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

        /// <summary>
        /// Applies the fade-in effect to the avatar animation based on the current progress of the fade-in timer.
        /// </summary>
        private void ApplyFadeIn()
        {
            Color color = Color.white.WithAlpha(_fadeInTimer.progress);
            SetMaterialColor(color);
        }

        /// <summary>
        /// Applies the fade-out effect to the avatar animation based on the current progress of the fade-out timer.
        /// </summary>
        private void ApplyFadeOut()
        {
            Color color = Color.white.WithAlpha(_fadeOutTimer.progress);
            SetMaterialColor(color);
        }

        /// <summary>
        /// Gathers the materials of renderers for color effects and stores them in the _materials list.
        /// </summary>
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
