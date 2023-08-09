using UnityEngine;
using System.Collections;
using egads.tools.extensions;
using egads.tools.objects;
using egads.tools.utils;
using egads.system.actions;
using egads.system.timer;
using egads.system.gameManagement;

namespace egads.system.actors
{
    /// <summary>
    /// A 2D actor class that implements IPooledObject, IActor, and IHasHealth interfaces.
    /// </summary>
    public class Character2D : MonoBehaviour, IPooledObject, ICharacter, IHasHealth
	{
        #region Constants

        /// <summary>
        /// Minimum movement threshold for triggering animations.
        /// </summary>
        public const float MINIMUM_MOVEMENT_FOR_ANIMATIONS = 0.01f;

        /// <summary>
        /// Distance to target considered as reached.
        /// </summary>
        public const float TARGET_DISTANCE = 0.5f;

        /// <summary>
        /// Time until the actor is destroyed after death.
        /// </summary>
        public const float TIME_UNTIL_DESTRUCTION = 10.0f;

        #endregion

        #region Delegates

        /// <summary>
        /// Delegate for estimating the future position of the actor after a given time.
        /// </summary>
        /// <param name="time">The time in seconds to estimate the future position for.</param>
        /// <returns>The estimated future position of the actor.</returns>
        public delegate Vector2 GetEstimatedFuturePositionDelegate(float time);

        /// <summary>
        /// Delegate for checking if there are possible targets within the actor's range.
        /// </summary>
        /// <returns>True if there are possible targets within range, false otherwise.</returns>
        public delegate bool HasPossibleTargetsInRangeDelegate();

        /// <summary>
        /// Delegate for handling the execution when the actor dies.
        /// </summary>
        public delegate void DeathExecutionDelegate();

        /// <summary>
        /// Delegate for handling when the actor is damaged.
        /// </summary>
        public delegate void WasDamagedDelegate();

        #endregion

        #region Events

        /// <summary>
        /// Event triggered to estimate the future position of the actor.
        /// </summary>
        public GetEstimatedFuturePositionDelegate GetEstimatedFuturePosition;


        /// <summary>
        /// Event triggered to check if there are possible targets within range of the actor.
        /// </summary>
        public HasPossibleTargetsInRangeDelegate HasPossibleTargetsInRange = delegate { return false; };

        /// <summary>
        /// Event triggered when the actor's death execution is required.
        /// </summary>
        public DeathExecutionDelegate deathExecutionHandler;

        /// <summary>
        /// Event triggered when the actor is damaged.
        /// </summary>
        public event WasDamagedDelegate wasDamaged;

        /// <summary>
        /// Event triggered when the state of the actor changes.
        /// </summary>
        public event StateChanged stateChanged;

        #endregion

        #region State

        [SerializeField]
		private CharacterState _state = CharacterState.Idle;

        /// <summary>
        /// The current state of the actor.
        /// </summary>
        public CharacterState state
		{
			get { return _state; }
			private set
			{
                // Ignore if the actor's transform is null (prevents unnecessary changes when the object is being destroyed).
                if (_transform == null) { return; }

                // If the state has not changed, do nothing.
                if (_state == value) { return; }

                // Set the new state.
                _state = value;

                // Trigger the stateChanged event if there are subscribers.
                if (stateChanged != null) { stateChanged(this, _state); }

                // Trigger the getsDisabled event if the state changes to Disabled and there are subscribers.
                if (_state == CharacterState.Disabled && getsDisabled != null) { getsDisabled(this); }
			}
		}

        #endregion

        #region Unit Stats, Health

        #region Stats

        [SerializeField]
        private float _maxHealth = 10.0f;

        private Energy _health;

        /// <summary>
        /// The current health of the actor.
        /// </summary>
        public Energy health
        {
            get
            {
                if (_health == null) { _health = new Energy(_maxHealth); }
                return _health;
            }
            set { _health = value; }
        }

        /// <summary>
        /// Returns true if the actor needs healing.
        /// </summary>
        public bool needsHealing => (isAlive && !health.isFull);

        public float movementSpeed = 6.0f;

        [HideInInspector]
        public float verticalMovementDampening = 1f;

        [HideInInspector]
        public CharacterTarget target;

        public Vector2 lookDirection = Vector2.zero;

        public Transform actionPivot;

        #endregion

        #region Global Values

        /// <summary>
        /// The color used for displaying healing effects.
        /// </summary>
        public static Color healingColor = new Color(0.58f, 0.91f, 0.82f);

        #endregion

        #region Getters and Setters

        /// <summary>
        /// The 2D position of the actor.
        /// </summary>
        public Vector2 position2D => _transform.position;

        /// <summary>
        /// The 3D position of the actor.
        /// </summary>
        public Vector3 position => _transform.position;

        /// <summary>
        /// Returns true if the actor is alive.
        /// </summary>
        public bool isAlive => !(state == CharacterState.Dead || state == CharacterState.Disabled);

        /// <summary>
        /// Returns true if the actor is enabled (not disabled).
        /// </summary>
        public bool isEnabled => !(state == CharacterState.Disabled);

        /// <summary>
        /// Returns true if the actor is ready to perform actions.
        /// </summary>
        public bool isReady => (state == CharacterState.Idle || state == CharacterState.Moving);

        #endregion

        #endregion

        #region Actions

        /// <summary>
        /// The timed action that this actor can perform.
        /// </summary>
        public ICharacterTimedAction action;

        #endregion

        #region Display

        /// <summary>
        /// The portrait displayed in the user interface for this actor.
        /// </summary>
        public Texture2D interfacePortrait;

        /// <summary>
        /// The GameObject representing the visual display of this actor.
        /// </summary>
        public GameObject displayObject;

        /// <summary>
        /// Indicates whether the actor is facing to the right direction.
        /// </summary>
        public bool directionRight = true;

        /// <summary>
        /// Indicates whether the actor can flip its visual display.
        /// </summary>
        public bool canFlip = true;

        /// <summary>
        /// Flag indicating if the actor is currently moving.
        /// </summary>
        [HideInInspector]
        public bool isMoving = false;

        /// <summary>
        /// The interface to control animations for this actor.
        /// </summary>
        private IAnimationController _animationController;

        // Variables for showing a hit (used for visual feedback when actor is damaged)
        private Timer _hitTimer = null;
        private Color _originalColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        #endregion

        #region Private Properties

        // Cached references to frequently accessed components
        private Transform _transform;
        private Rigidbody2D _rigidbody2D;
        private Collider2D _collider2D;
        private Timer _actionTimer = null;

        // Cached values for movement and input handling
        private Vector2 _lastMovement = Vector2.zero;
        private Vector2 _lastDirectMovement = Vector2.zero;
        private int _gotDirectMovementInput = 0;

        #endregion

        #region Unity methods

        /// <summary>
        /// Called when the GameObject is initialized.
        /// Initializes references to components and sets up the actor's health and target.
        /// </summary>
        void Awake()
        {
            _transform = transform;
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _collider2D = GetComponent<Collider2D>();
            _animationController = _transform.GetInterface<IAnimationController>();
            target = new CharacterTarget(this, _transform);

            if (_health == null) { health = new Energy(_maxHealth); }

            // Set callback functions
            GetEstimatedFuturePosition = EstimateFuturePosition;
            deathExecutionHandler = DestroyAtDeath;

            Reset();
        }

        /// <summary>
        /// Called once per frame.
        /// Stops direct movement input if there is no target and no more direct input commands.
        /// </summary>
        void Update()
        {
            // We use this so direct movement input gets stopped at some point if there is silent communication from the input side
            if (_gotDirectMovementInput > 0 && target.hasTarget)
            {
                _gotDirectMovementInput--;

                if (_gotDirectMovementInput == 0) { StopMovement(); }
            }
        }

        /// <summary>
        /// Called in fixed intervals (physics updates).
        /// Handles slow movement damping, hit visuals timer, target updates, movement updates, and action updates.
        /// </summary>
        void FixedUpdate()
        {
            // Do nothing when the game is not running
            if (MainBase.Instance == null || (MainBase.Instance.state != GameState.Running && MainBase.Instance.state != GameState.Sequence)) { return; }

            // Slow movement damping
            if (_rigidbody2D != null)
            {
                if (_rigidbody2D.velocity.magnitude < 0.005f)
                {
                    _rigidbody2D.velocity = Vector2.zero;
                    lookDirection = Vector2.zero;
                }
                else { _rigidbody2D.velocity = Vector2.Lerp(_rigidbody2D.velocity, Vector2.zero, Time.fixedDeltaTime * 5.0f); }
            }

            // Do nothing when dead
            if (state == CharacterState.Disabled || state == CharacterState.Dead) { return; }

            // Timer for hit visuals
            if (_hitTimer != null && isAlive)
            {
                _hitTimer.Update();

                if (_hitTimer.hasEnded) { HideDamageDisplay(); }
            }

            // Update target
            target.Update();

            // Update movement
            if (state == CharacterState.Moving)
            {
                if (!target.hasTarget)
                {
                    if (_lastDirectMovement != Vector2.zero && movementSpeed > 0) { Move(_lastDirectMovement); }
                    else
                    {
                        isMoving = false;
                        state = CharacterState.Idle;
                        return;
                    }
                }
                else { MoveTowardsTarget(); }
            }

            // Update action
            if (_actionTimer != null)
            {
                _actionTimer.Update();
                if (_actionTimer.hasEnded)
                {
                    state = CharacterState.Idle;
                    _actionTimer = null;
                }
                else { return; } // Wait for actions to finish
            }

            // Resume actions when idle and has a target
            if (state == CharacterState.Idle && target.hasTarget)
            {
                state = CharacterState.Moving;
                return;
            }
        }

        #endregion

        #region Public Methods

        #region State Manipulation

        /// <summary>
        /// Makes the actor inactive by disabling its target and setting its state to Disabled.
        /// </summary>
        public void MakeInactive()
        {
            target.DisableTarget();
            state = CharacterState.Disabled;
        }

        /// <summary>
        /// Disables the actor by making it inactive and optionally destroying its GameObject.
        /// </summary>
        public void Disable()
        {
            MakeInactive();

            if (!_isInactiveInObjectPool) { Destroy(gameObject); }
        }

        /// <summary>
        /// Disables the actor, fades out its animation over the specified time, and optionally destroys its GameObject.
        /// </summary>
        /// <param name="time">The time in seconds over which to fade out the animation before disabling the actor.</param>
        public void DisableAndFadeOut(float time)
        {
            MakeInactive();

            if (_animationController != null) { _animationController.FadeOut(time); }

            if (!_isInactiveInObjectPool) { Destroy(gameObject, time); }
        }

        /// <summary>
        /// Kills the actor by applying damage equal to its current health, effectively reducing its health to zero.
        /// </summary>
        public void Kill()
        {
            ApplyDamage(health.current);
        }

        /// <summary>
        /// Resets the actor to its initial state by resetting its health, disabling its target, and setting its state to Idle.
        /// </summary>
        public void Reset()
        {
            health.Reset();
            if (target != null) { target.DisableTarget(); }

            if (_collider2D != null) { _collider2D.enabled = true; }

            if (_animationController != null) { _animationController.Reset(); }

            HideDamageDisplay();

            state = CharacterState.Idle;
        }

        /// <summary>
        /// Applies damage to the actor, reducing its health by the specified amount.
        /// Triggers death if the health becomes empty, and shows damage display.
        /// </summary>
        /// <param name="damage">The amount of damage to apply to the actor.</param>
        public void ApplyDamage(float damage)
        {
            if (!isAlive) { return; }

            health.Lose(damage);

            if (health.isEmpty) { Die(); }
            else { ShowDamageDisplay(0.15f, new Color(1.0f, 0.4f, 0.4f, 1.0f)); }

            if (wasDamaged != null) { wasDamaged(); }
        }

        /// <summary>
        /// Applies healing to the actor, increasing its health by the specified amount.
        /// Shows a healing damage display.
        /// </summary>
        /// <param name="amount">The amount of healing to apply to the actor.</param>
        public void ApplyHealing(float amount)
        {
            if (!isAlive) { return; }

            ShowDamageDisplay(0.25f, healingColor);

            health.Add(amount);
        }

        #endregion

        #region Target Setting

        /// <summary>
        /// Sets a new target for the actor as a Transform, such as walking to a building or a flag.
        /// </summary>
        /// <param name="newTarget">The new target Transform to set.</param>
        /// <param name="distance">The distance at which the actor should stop when reaching the target (default is TARGET_DISTANCE).</param>
        /// <param name="determined">Indicates whether the actor's movement is determined or not (default is false).</param>
        public void SetTarget(Transform newTarget, float distance = TARGET_DISTANCE, bool determined = false)
        {
            if (!isAlive) { return; }

            target.SetTarget(newTarget, distance, determined);

            if (state == CharacterState.Idle) { state = CharacterState.Moving; }
        }

        /// <summary>
        /// Sets a new target for the actor as a position in 2D space.
        /// </summary>
        /// <param name="position">The new target position to set.</param>
        /// <param name="distance">The distance at which the actor should stop when reaching the target (default is TARGET_DISTANCE).</param>
        /// <param name="determined">Indicates whether the actor's movement is determined or not (default is false).</param>
        public void SetTarget(Vector2 position, float distance = TARGET_DISTANCE, bool determined = false)
        {
            if (!isAlive) { return; }

            target.SetTarget(position, distance, determined);

            if (state == CharacterState.Idle) { state = CharacterState.Moving; }
        }

        /// <summary>
        /// Sets another actor as the actor's new target, such as an enemy or friendly unit to be healed.
        /// </summary>
        /// <param name="otherActor">The other actor to set as the target.</param>
        /// <param name="determined">Indicates whether the actor's movement is determined or not (default is false).</param>
        public void SetTarget(Character2D otherActor, bool determined = false)
        {
            if (!isAlive) { return; }

            if (action != null) { target.SetTarget(otherActor, action.range * 0.9f, determined); }

            if (state == CharacterState.Idle) { state = CharacterState.Moving; }
        }

        /// <summary>
        /// Sets the actor's movement direction using a 2D Vector.
        /// </summary>
        /// <param name="moveDirection">The direction in which the actor should move.</param>
        public void SetMovement(Vector2 moveDirection)
        {
            if (!isAlive) { return; }

            target.DisableTarget();

            if (moveDirection == Vector2.zero)
            {
                _lastDirectMovement = Vector2.zero;
                StopMovement();
                return;
            }

            if (moveDirection.sqrMagnitude > 1.0f) { moveDirection = moveDirection.normalized; }

            _gotDirectMovementInput = 4;
            _lastDirectMovement = moveDirection;

            Move(moveDirection);
            if (moveDirection.x > 0) { SetHorizontalDisplayDirection(true); }
            else if (moveDirection.x < 0) { SetHorizontalDisplayDirection(false); }

            if (state == CharacterState.Idle) { state = CharacterState.Moving; }
        }

        /// <summary>
        /// Makes the actor stand still at the current position and prevents it from being moved around by other actors.
        /// Used when in a passive state, such as during dialogue.
        /// </summary>
        public void Freeze()
        {
            StopMovement();
            _rigidbody2D.isKinematic = true;
        }

        /// <summary>
        /// Unfreezes the actor, allowing it to move and be affected by physics.
        /// </summary>
        public void UnFreeze()
        {
            _rigidbody2D.isKinematic = false;
        }

        /// <summary>
        /// Stops the actor's movement, disables the target, and sets the state to Idle.
        /// </summary>
        public void StopMovement()
        {
            if (isAlive)
            {
                _lastDirectMovement = Vector2.zero;

                isMoving = false;
                target.DisableTarget();

                if (state == CharacterState.Moving) { state = CharacterState.Idle; }
            }
        }

        /// <summary>
        /// Initiates the actor to take an action on a target actor, such as attacking or using a skill.
        /// </summary>
        /// <param name="targetActor">The target actor on which the action will be performed.</param>
        public void TakeAction(Character2D targetActor)
        {
            if (!isAlive) { return; }

            _actionTimer = new Timer(action.cooldown);
            isMoving = false;

            // Update look direction
            SetLookDirectionToTarget(targetActor.position2D);

            state = CharacterState.TakingAction;

            action.Execute();
        }

        /// <summary>
        /// Initiates the actor to take an action as part of an IEnumeratedAction sequence.
        /// </summary>
        /// <param name="enumeratedAction">The IEnumeratedAction representing the action sequence.</param>
        public void TakeAction(IEnumeratedAction enumeratedAction)
        {
            isMoving = false;
            state = CharacterState.TakingAction;

            StartCoroutine(StartAction(enumeratedAction));
        }

        private IEnumerator StartAction(IEnumeratedAction enumeratedAction)
        {
            yield return StartCoroutine(enumeratedAction.Execute());
            state = CharacterState.Idle;
        }

        #endregion

        #endregion

        #region Private Methods

        #region Movement and Actions

        /// <summary>
        /// Handles collision detection and calculates avoidance when colliding with other objects while moving.
        /// </summary>
        /// <param name="coll">The Collision2D data representing the collision.</param>
        private void OnCollisionEnter2D(Collision2D coll)
        {
            // Only relevant for moving objects which have a target
            if (movementSpeed <= 0 || _rigidbody2D == null || target == null || !target.hasTarget) { return; }

            Vector2 targetLocation = target.GetCurrentTargetLocation();
            Vector2 moveDirection = Utilities2D.GetNormalizedDirection(position2D, targetLocation);

            if (moveDirection.sqrMagnitude <= 0.03f) { return; }
            moveDirection = moveDirection.normalized;

            // Get contact
            ContactPoint2D contactPoint = coll.contacts[0];

            // Calculate force to the right or to the left
            Vector2 toTheRight = new Vector2(moveDirection.y, -moveDirection.x);
            Vector2 toTheLeft = -toTheRight;
            float angle = Vector2.Angle(contactPoint.normal, toTheRight);
            Vector2 avoidanceVector;

            if (angle > 90.0f) { avoidanceVector = toTheLeft; }
            else { avoidanceVector = toTheRight; }

            _rigidbody2D.AddForce(avoidanceVector * 30.0f);
        }

        /// <summary>
        /// Moves the actor towards its current target location.
        /// </summary>
        private void MoveTowardsTarget()
        {
            // Check if target reached
            if (target.isReached)
            {
                // Attack if possible
                if (target.type == CharacterTarget.TargetType.Actor && TargetInReach() && action != null)
                {
                    TakeAction(target.otherActor);
                    return;
                }

                StopMovement();
                return;
            }

            // Get target direction
            Vector2 targetLocation = target.GetCurrentTargetLocation();

            Vector2 moveDirection = Utilities2D.GetNormalizedDirection(position2D, targetLocation);

            Move(moveDirection);

            // Update look direction
            SetLookDirectionToTarget(targetLocation);
        }

        // Hack; should not be necessary to double check this
        private bool TargetInReach()
        {
            Vector2 direction = position2D - target.otherActor.position2D;
            return direction.magnitude <= action.range;
        }

        /// <summary>
        /// Handles the movement of the actor based on the provided move direction.
        /// </summary>
        /// <param name="moveDirection">The direction in which the actor should move.</param>
        private void Move(Vector2 moveDirection)
        {
            // Movement
            Vector2 movement = moveDirection * movementSpeed;
            movement.y *= verticalMovementDampening;
            _rigidbody2D.velocity = movement;

            // Mark controller as moving or not (for animations)
            if (movement.magnitude > MINIMUM_MOVEMENT_FOR_ANIMATIONS)
            {
                lookDirection = movement;
                isMoving = true;
            }
            else { isMoving = false; }

            // Save movement value to calculate future position for targeting this Actor
            _lastMovement = movement;
        }

        /// <summary>
        /// Estimates the future position of the actor based on the provided time.
        /// </summary>
        /// <param name="time">The time in seconds to estimate the future position.</param>
        /// <returns>The estimated future position of the actor.</returns>
        private Vector2 EstimateFuturePosition(float time)
        {
            Vector2 futurePos = position2D;

            if (isAlive && isMoving)
            {
                Vector2 direction = _lastMovement.normalized;

                if (target.hasTarget) { direction = Utilities2D.GetNormalizedDirection(futurePos, target.GetFinalTargetPosition()); }

                futurePos = futurePos + direction * time * movementSpeed;
            }

            return futurePos;
        }

        /// <summary>
        /// Handles the death of the actor, triggers necessary actions, and initiates destruction if required.
        /// </summary>
        private void Die()
        {
            // Reset hit display
            HideDamageDisplay();

            state = CharacterState.Dead;

            deathExecutionHandler();
        }

        /// <summary>
        /// Initiates destruction of the actor after a specified time when it dies.
        /// </summary>
        private void DestroyAtDeath()
        {
            if (_animationController != null) { _animationController.FadeOutAfterDeath(); }

            // Disable components
            if (_collider2D != null) { _collider2D.enabled = false; }

            // Deathtime
            float timeUntilDestroy = TIME_UNTIL_DESTRUCTION;

            if (_isInactiveInObjectPool) { StartCoroutine(WaitAndDisable(timeUntilDestroy)); }
            else { Destroy(gameObject, timeUntilDestroy); }
        }

        /// <summary>
        /// Waits for a specified time and then disables the actor.
        /// </summary>
        /// <param name="time">The time in seconds to wait before disabling the actor.</param>
        /// <returns>An IEnumerator for use in a Coroutine.</returns>
        private IEnumerator WaitAndDisable(float time)
        {
            yield return new WaitForSeconds(time);
            Disable();
        }

        #endregion

        #region Display

        /// <summary>
        /// Sets the horizontal display direction of the actor based on whether it should face right or left.
        /// </summary>
        /// <param name="toTheRight">Indicates whether the actor should face right (true) or left (false).</param>
        public void SetHorizontalDisplayDirection(bool toTheRight)
        {
            if (toTheRight != directionRight) { FlipDirection(); }
        }

        /// <summary>
        /// Sets the look direction of the actor.
        /// </summary>
        /// <param name="direction">The direction to set as the look direction.</param>
        public void SetLookDirection(Vector2 direction)
        {
            lookDirection = direction;
            if (direction.x > 0) { SetHorizontalDisplayDirection(true); }
            else if (direction.x < 0) { SetHorizontalDisplayDirection(false); }
        }

        /// <summary>
        /// Sets the look direction of the actor towards a target location.
        /// </summary>
        /// <param name="targetLocation">The target location to set as the look direction.</param>
        private void SetLookDirectionToTarget(Vector2 targetLocation)
        {
            SetLookDirection(targetLocation - position2D);
        }

        /// <summary>
        /// Flips the direction of the actor, changing its visual avatar's orientation.
        /// </summary>
        private void FlipDirection()
        {
            directionRight = !directionRight;

            if (!canFlip) { return; }

            // Change visual avatar
            if (displayObject != null)
            {
                Transform displayTransform = displayObject.transform;
                displayTransform.localScale = new Vector3(displayTransform.localScale.x * -1.0f, displayTransform.localScale.y, displayTransform.localScale.z);
            }
        }

        /// <summary>
        /// Displays the damage taken by the actor for a specified time and color.
        /// </summary>
        /// <param name="time">The time in seconds to display the damage.</param>
        /// <param name="color">The color of the damage display.</param>
        private void ShowDamageDisplay(float time, Color color)
        {
            _hitTimer = new Timer(time);

            if (_animationController != null) { _animationController.SetMaterialColor(color); }
        }

        /// <summary>
        /// Hides the damage display, resetting the material color to its original state.
        /// </summary>
        private void HideDamageDisplay()
        {
            if (_hitTimer != null)
            {
                _hitTimer = null;

                if (_animationController != null) { _animationController.SetMaterialColor(_originalColor); }
            }
        }

        #endregion

        #endregion

        #region Interface Implementation

        public event System.Action<IPooledObject> getsDisabled;
        private bool _isInactiveInObjectPool = false;
        public bool isUsedByObjectPool
        {
            get { return _isInactiveInObjectPool; }
            set { _isInactiveInObjectPool = value; }
        }

        /// <summary>
        /// Activates the actor, resetting its state to default.
        /// </summary>
        public void ToggleOn()
        {
            gameObject.SetActive(true);
            Reset();
        }

        /// <summary>
        /// Deactivates the actor, stopping all coroutines and disabling its components.
        /// </summary>
        public void ToggleOff()
        {
            StopAllCoroutines();
            Disable();
            gameObject.SetActive(false);
        }

        #endregion

    }
}

