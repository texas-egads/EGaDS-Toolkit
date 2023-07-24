using UnityEngine;
using System.Collections;
using egads.tools.extensions;
using egads.tools.objects;
using egads.tools.utils;
using egads.system.actions;
using egads.system.timer;
using egads.system.gameManagement;
using TMPro;

namespace egads.system.actors
{
	public class Actor2D : MonoBehaviour, IPooledObject, IActor, IHasHealth
	{
        #region Constants

        public const float MINIMUM_MOVEMENT_FOR_ANIMATIONS = 0.01f;
        public const float TARGET_DISTANCE = 0.5f;
        public const float TIME_UNTIL_DESTRUCTION = 10.0f;

        #endregion

        #region Delegates

        public delegate Vector2 GetEstimatedFuturePositionDelegate(float time);
        public delegate bool HasPossibleTargetsInRangeDelegate();
        public delegate void DeathExecutionDelegate();
        public delegate void WasDamagedDelegate();

        #endregion

        #region Events

        public GetEstimatedFuturePositionDelegate GetEstimatedFuturePosition;

		
		public HasPossibleTargetsInRangeDelegate HasPossibleTargetsInRange = delegate { return false; };

		
		public DeathExecutionDelegate deathExecutionHandler;

		
		public event WasDamagedDelegate wasDamaged;

		public event StateChanged stateChanged;

		#endregion

		#region State

		[SerializeField]
		private ActorState _state = ActorState.Idle;
		public ActorState state
		{
			get { return _state; }
			private set
			{
				if (_transform == null) { return; }

				if (_state == value) { return; }

				_state = value;

				if (stateChanged != null) { stateChanged(this, _state); }

				if (_state == ActorState.Disabled && getsDisabled != null) { getsDisabled(this); }
			}
		}

        #endregion

        #region Unit Stats, Health

        #region Stats

        [SerializeField]
		private float _maxHealth = 10.0f;

		private Energy _health;
		public Energy health
		{
			get
			{
				if (_health == null) { _health = new Energy(_maxHealth); }
				return _health;
			}
			set { _health = value; }
		}

		public bool needsHealing => (isAlive && !health.isFull);

		public float movementSpeed = 6.0f;

		[HideInInspector]
		public float verticalMovementDampening = 1f;

		[HideInInspector]
		public ActorTarget target;

		public Vector2 lookDirection = Vector2.zero;

		public Transform actionPivot;

        #endregion

        #region Global Values

        public static Color healingColor = new Color(0.58f, 0.91f, 0.82f);

        #endregion

        #region Getters and Setters

        public Vector2 position2D => _transform.position;
		public Vector3 position => _transform.position;

		public bool isAlive => !(state == ActorState.Dead || state == ActorState.Disabled);

		public bool isEnabled => !(state == ActorState.Disabled);

		public bool isReady => (state == ActorState.Idle || state == ActorState.Moving);

        #endregion

        #endregion

        #region Actions

        public IActorTimedAction action;

		#endregion

		#region Display

		public Texture2D interfacePortrait;
		public GameObject displayObject;
		public bool directionRight = true;
		public bool canFlip = true;

		[HideInInspector]
		public bool isMoving = false;

		private IAnimationController _animationController;

		// variables for showing a hit
		private Timer _hitTimer = null;
		private Color _originalColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

		#endregion

		#region Private Properties

		private Transform _transform;
		private Rigidbody2D _rigidbody2D;
		private Collider2D _collider2D;
		private Timer _actionTimer = null;

		private Vector2 _lastMovement = Vector2.zero;
		private Vector2 _lastDirectMovement = Vector2.zero;
		private int _gotDirectMovementInput = 0;

		#endregion

		#region Unity methods

		void Awake()
		{
			_transform = transform;
			_rigidbody2D = GetComponent<Rigidbody2D>();
			_collider2D = GetComponent<Collider2D>();
			_animationController = _transform.GetInterface<IAnimationController>();
			target = new ActorTarget(this, _transform);

			if (_health == null)
				health = new Energy(_maxHealth);

			// Set callback functions
			GetEstimatedFuturePosition = EstimateFuturePosition;
			deathExecutionHandler = DestroyAtDeath;

			Reset();
		}

		void Update()
		{
			// We use this so direct movement input gets stopped at some point if there is silent communication from the input side
			if (_gotDirectMovementInput > 0 && target.hasTarget)
			{
				_gotDirectMovementInput--;

				if (_gotDirectMovementInput == 0)
				{
					StopMovement();
				}
			}
		}

		void FixedUpdate()
		{
			// Do nothing when game is not running
			if (MainBase.Instance == null || (MainBase.Instance.state != GameState.Running && MainBase.Instance.state != GameState.Sequence)) { return; }

			// Slow movement
			if (_rigidbody2D != null)
			{
				if (_rigidbody2D.velocity.magnitude < 0.005f)
				{
					_rigidbody2D.velocity = Vector2.zero;
					lookDirection = Vector2.zero;
				}
				else
				{
					_rigidbody2D.velocity = Vector2.Lerp(_rigidbody2D.velocity, Vector2.zero, Time.fixedDeltaTime * 5.0f);
				}
			}

			// Do nothing when dead
			if (state == ActorState.Disabled || state == ActorState.Dead)
				return;

			// Timer for hit visuals
			if (_hitTimer != null && isAlive)
			{
				_hitTimer.Update();

				if (_hitTimer.hasEnded)
				{
					HideDamageDisplay();
				}
			}

			// Update target
			target.Update();

			// Update movement
			if (state == ActorState.Moving)
			{
				if (!target.hasTarget)
				{
					if (_lastDirectMovement != Vector2.zero && movementSpeed > 0)
					{
						Move(_lastDirectMovement);
					}
					else
					{
						isMoving = false;
						state = ActorState.Idle;
						return;
					}
				}
				else
				{
					MoveTowardsTarget();
				}
			}

			// Update action
			if (_actionTimer != null)
			{
				_actionTimer.Update();
				if (_actionTimer.hasEnded)
				{
					state = ActorState.Idle;
					_actionTimer = null;
				}
				else
				{
					return; // Wait for actions to finish
				}
			}

			// Resume actions when idle
			if (state == ActorState.Idle && target.hasTarget)
			{
				state = ActorState.Moving;
				return;
			}
		}

        #endregion

        #region Public Methods

        #region State Manipulation

        public void MakeInactive()
		{
			target.DisableTarget();
			state = ActorState.Disabled;
		}

		public void Disable()
		{
			MakeInactive();

			if (!_isInactiveInObjectPool) { Destroy(gameObject); }
		}

		public void DisableAndFadeOut(float time)
		{
			MakeInactive();

			if (_animationController != null) { _animationController.FadeOut(time); }
				
			if (!_isInactiveInObjectPool) { Destroy(gameObject, time); }		
		}

		public void Kill()
		{
			ApplyDamage(health.current);
		}

		public void Reset()
		{
			health.Reset();
			if (target != null) { target.DisableTarget(); }
				
			if (_collider2D != null) { _collider2D.enabled = true; }
				

			if (_animationController != null) { _animationController.Reset(); }

			HideDamageDisplay();

			state = ActorState.Idle;
		}

		// Decrease health, can be called from outside
		public void ApplyDamage(float damage)
		{
			if (!isAlive)
				return;

			health.Lose(damage);

			if (health.isEmpty) { Die(); }
			else { ShowDamageDisplay(0.15f, new Color(1.0f, 0.4f, 0.4f, 1.0f)); }

			if (wasDamaged != null) { wasDamaged(); }
		}

		// Increase health, can be called from outside
		public void ApplyHealing(float amount)
		{
			if (!isAlive) { return; }

			ShowDamageDisplay(0.25f, healingColor);

			health.Add(amount);
		}

		#endregion

		#region Target Setting

		// Transform as new target, e.g. walking to a building, flag
		public void SetTarget(Transform newTarget, float distance = TARGET_DISTANCE, bool determined = false)
		{
			if (!isAlive) { return; }

			target.SetTarget(newTarget, distance, determined);

			if (state == ActorState.Idle) { state = ActorState.Moving; }
		}

		// Position as new target
		public void SetTarget(Vector2 position, float distance = TARGET_DISTANCE, bool determined = false)
		{
			if (!isAlive) { return; }

			target.SetTarget(position, distance, determined);

			if (state == ActorState.Idle) { state = ActorState.Moving; }	
		}

		// Other actor as new target, e.g. enemy or friendly unit to be healed
		public void SetTarget(Actor2D otherActor, bool determined = false)
		{
			if (!isAlive) { return; }

			if (action != null) { target.SetTarget(otherActor, action.range * 0.9f, determined); }

			if (state == ActorState.Idle) { state = ActorState.Moving; }
		}

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

			if (state == ActorState.Idle) { state = ActorState.Moving; }
		}

		/// <summary>
		/// Stand still at current position and don't get moved around by other actors
		/// Used when in a passive state like dialogue
		/// </summary>
		public void Freeze()
		{
			StopMovement();
			_rigidbody2D.isKinematic = true;
		}

		public void UnFreeze()
		{
			_rigidbody2D.isKinematic = false;
		}

		public void StopMovement()
		{
			if (isAlive)
			{
				_lastDirectMovement = Vector2.zero;

				isMoving = false;
				target.DisableTarget();

				if (state == ActorState.Moving)
					state = ActorState.Idle;
			}
		}

		public void TakeAction(Actor2D targetActor)
		{
			if (!isAlive)
				return;

			_actionTimer = new Timer(action.cooldown);
			isMoving = false;

			// Update look direction
			SetLookDirectionToTarget(targetActor.position2D);

			state = ActorState.TakingAction;

			action.Execute();
		}

		public void TakeAction(IEnumeratedAction enumeratedAction)
		{
			isMoving = false;
			state = ActorState.TakingAction;

			StartCoroutine(StartAction(enumeratedAction));
		}

		private IEnumerator StartAction(IEnumeratedAction enumeratedAction)
		{
			yield return StartCoroutine(enumeratedAction.Execute());
			state = ActorState.Idle;
		}

        #endregion

        #endregion

        #region Private Methods

        #region Movement and Actions

        /// <summary>
        /// Calculate avoidance on collision
        /// </summary>
        /// <param name="coll"></param>
        private void OnCollisionEnter2D(Collision2D coll)
		{
			// Only relevant for moving objects which have a target
			if (movementSpeed <= 0 || _rigidbody2D == null || target == null || !target.hasTarget) { return; }

			Vector2 targetLocation = target.GetCurrentTargetLocation();
			Vector2 moveDirection = Utilities2D.GetNormalizedDirection(position2D, targetLocation);

			if (moveDirection.sqrMagnitude <= 0.03f)
				return;
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

		private void MoveTowardsTarget()
		{
			// Check if target reached
			if (target.isReached)
			{
				// Attack if possible
				if (target.type == ActorTarget.TargetType.Actor && TargetInReach() && action != null)
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
		/// Handles Movement of actor, change this to fit your movement needs
		/// </summary>
		/// <param name="moveDirection"></param>
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

		private Vector2 EstimateFuturePosition(float time)
		{
			Vector2 futurePos = position2D;

			if (isAlive && isMoving)
			{
				Vector2 direction = _lastMovement.normalized;

				if (target.hasTarget)
				{
					direction = Utilities2D.GetNormalizedDirection(futurePos, target.GetFinalTargetPosition());
				}

				futurePos = futurePos + direction * time * movementSpeed;
			}

			return futurePos;
		}

		private void Die()
		{
			// Reset hit display
			HideDamageDisplay();

			state = ActorState.Dead;

			deathExecutionHandler();
		}

		private void DestroyAtDeath()
		{
			if (_animationController != null)
				_animationController.FadeOutAfterDeath();

			// Disable components
			if (_collider2D != null)
				_collider2D.enabled = false;

            // Deathtime
            float timeUntilDestroy = TIME_UNTIL_DESTRUCTION; 

			if (_isInactiveInObjectPool)
			{
				StartCoroutine(WaitAndDisable(timeUntilDestroy));
			}
			else
				Destroy(gameObject, timeUntilDestroy);
		}

		private IEnumerator WaitAndDisable(float time)
		{
			yield return new WaitForSeconds(time);
			Disable();
		}

		#endregion

		#region Display

		public void SetHorizontalDisplayDirection(bool toTheRight)
		{
			if (toTheRight != directionRight) { FlipDirection(); }
		}

		public void SetLookDirection(Vector2 direction)
		{
			lookDirection = direction;
			if (direction.x > 0) { SetHorizontalDisplayDirection(true); }
			else if (direction.x < 0) { SetHorizontalDisplayDirection(false); }
		}

		private void SetLookDirectionToTarget(Vector2 targetLocation)
		{
			SetLookDirection(targetLocation - position2D);
		}

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

		private void ShowDamageDisplay(float time, Color color)
		{
			_hitTimer = new Timer(time);

			if (_animationController != null) { _animationController.SetMaterialColor(color); }	
		}

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

        public void ToggleOn()
		{
			gameObject.SetActive(true);
			Reset();
		}

		public void ToggleOff()
		{
			StopAllCoroutines();
			Disable();
			gameObject.SetActive(false);
		}

        #endregion
    }
}