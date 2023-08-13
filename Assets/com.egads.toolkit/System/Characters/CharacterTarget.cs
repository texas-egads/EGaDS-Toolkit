using UnityEngine;
using egads.tools.utils;
using egads.system.pathFinding;

namespace egads.system.characters
{
    /// <summary>
    /// Represents a target for an Character2D to follow, such as a position, another Character, or a Transform.
    /// </summary>
    public class CharacterTarget
    {
        #region Constants

        /// <summary>
        /// The squared distance threshold for considering the character to have reached a path node.
        /// </summary>
        const float PATH_NODE_DISTANCE_SQUARED = 0.05f;

        #endregion

        #region Types

        /// <summary>
        /// The type of the target to follow.
        /// </summary>
        public enum TargetType
        {
            /// <summary>
            /// The target is a Vector2 position.
            /// </summary>
            Position,

            /// <summary>
            /// The target is a Transform object.
            /// </summary>
            Transform,

            /// <summary>
            /// The target is another Character2D.
            /// </summary>
            Character,

            /// <summary>
            /// There is no target.
            /// </summary>
            None
        }

        /// <summary>
        /// The type of event triggered when the character reaches or cannot reach the target.
        /// </summary>
        public enum TargetEventType
        {
            /// <summary>
            /// The character successfully reached the target.
            /// </summary>
            TargetReached,

            /// <summary>
            /// The character cannot reach the target, usually when the target character is no longer alive.
            /// </summary>
            CannotReachTarget
        }

        /// <summary>
        /// The target type assigned to this CharacterTarget.
        /// </summary>
        public TargetType type;

        #endregion

        #region Events

        /// <summary>
        /// Delegate for the target event, used for subscribing to target-related events.
        /// </summary>
        /// <param name="type">The type of target event.</param>
        public delegate void TargetEventDelegate(TargetEventType type);

        /// <summary>
        /// Event triggered when the character reaches or cannot reach the target.
        /// </summary>
        public event TargetEventDelegate targetEvent;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether the character has reached its target.
        /// </summary>
        public bool isReached => _isReached;

        /// <summary>
        /// Gets a value indicating whether the character has a target assigned.
        /// </summary>
        public bool hasTarget => (type != TargetType.None);

        /// <summary>
        /// Gets a value indicating whether the character's target is another character.
        /// </summary>
        public bool hasCharacterTarget => type == TargetType.Character;

        #endregion

        #region Private Properties

        // Flag indicating if the character has reached its target.
        private bool _isReached = true;

        // The Transform of the Character2D instance who is following the target.
        private Transform _protagonistTransform;

        // The Character2D instance who is following the target.
        private Character2D _protagonist;

        // The target position to follow (used when the target type is Position).
        private Vector2 _targetPosition;

        // The target Transform to follow (used when the target type is Transform or Character2D).
        private Transform _targetTransform;

        // The other Character2D target (used when the target type is Character2D).
        private Character2D _otherCharacter;

        /// <summary>
        /// Gets the other Character2D target when the target type is Character2D.
        /// </summary>
        public Character2D otherCharacter => _otherCharacter;

        // Saving squared value of target reached distance for performance reasons when comparing vector length.
        private float _targetReachedDistanceSquared;

        // The target reached distance (squared), used to determine if the target is reached.
        private float _targetReachedDistance;

        /// <summary>
        /// Gets or sets the distance at which the Character2D is considered to have reached the target.
        /// </summary>
        public float targetReachedDistance
        {
            get { return _targetReachedDistance; }
            private set
            {
                _targetReachedDistance = value;
                _targetReachedDistanceSquared = value * value;
            }
        }

        // Flag indicating if the target has been determined and this setting should override the previous one.
        private bool _determined = false;

        // The pathfinding field used for calculating the path to the target.
        private IPathField _pathField = null;

        // The path for pathfinding to the target.
        private Vector2Path _path = null;

        // Flag indicating if there is a valid path to the target.
        private bool _hasPath = false;

        // Counter used for performance optimizations, calculating path only every few frames.
        private int _ticker = 0;
        private int _maxTicker = 10;

        // Flag indicating if the path should be calculated in the next frame.
        private bool _calculatePathAtNextPossibility = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the CharacterTarget class.
        /// </summary>
        /// <param name="protagonist">The Character2D instance to be controlled by this target.</param>
        /// <param name="protagonistTransform">The Transform component of the Character2D instance.</param>
        public CharacterTarget(Character2D protagonist, Transform protagonistTransform)
        {
            _protagonist = protagonist;
            _protagonistTransform = protagonistTransform;

            // Randomly set the ticker to stagger path calculations for performance reasons.
            _ticker = Random.Range(0, _maxTicker);

            // Initialize the path with a buffer size of 80.
            _path = new Vector2Path(80);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates the CharacterTarget, making the Character2D follow the target if applicable.
        /// </summary>
        public void Update()
        {
            // Make sure we don't follow a null target character.
            if (type == TargetType.Character && _otherCharacter == null)
            {
                DisableTarget();
                return;
            }

            // Performance heavy calculations only every few frames.
            if (!_isReached) { PerformanceTicker(); }

            if (type != TargetType.None)
            {
                Vector2 targetPos = GetFinalTargetPosition();

                // Check if the Character2D has reached the target.
                if (Utilities2D.Vector2SqrDistance(_protagonistTransform.position, targetPos) <= _targetReachedDistanceSquared) { ReachTarget(); }
                else { _isReached = false; }

                // Check for the next target on the path.
                if (_hasPath && !_isReached)
                {
                    if (Utilities2D.Vector2SqrDistance(_protagonistTransform.position, GetCurrentTargetLocation()) <= PATH_NODE_DISTANCE_SQUARED)
                    {
                        NextNodeInPath();
                        if (!_hasPath)
                        {
                            _calculatePathAtNextPossibility = false;
                            ReachTarget();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the final target position, depending on the type of the target.
        /// </summary>
        /// <returns>The final target position as a Vector2.</returns>
        public Vector2 GetFinalTargetPosition()
        {
            Vector2 targetPos = _targetPosition;

            if (type == TargetType.Transform || type == TargetType.Character) { targetPos = _targetTransform.position; }

            return targetPos;
        }

        /// <summary>
        /// Gets the current target location, considering any ongoing path following.
        /// </summary>
        /// <returns>The current target location as a Vector2.</returns>
        public Vector2 GetCurrentTargetLocation()
        {
            if (_hasPath && !_path.hasFinished) { return _path.CurrentPosition; }
            else { return GetFinalTargetPosition(); }
        }

        /// <summary>
        /// Sets the target as a Vector2 position.
        /// </summary>
        /// <param name="targetPos">The target position as a Vector2.</param>
        /// <param name="targetDistance">The distance at which the Character2D is considered to have reached the target.</param>
        /// <param name="newDetermination">Optional. If true, this setting will override the previous one.</param>
        public void SetTarget(Vector2 targetPos, float targetDistance, bool newDetermination = false)
        {
            if (_determined == true && newDetermination == false) { return; }

            DisableTarget();

            type = TargetType.Position;
            _targetPosition = targetPos;
            targetReachedDistance = targetDistance;
            _isReached = false;
            _determined = newDetermination;

            _calculatePathAtNextPossibility = true;
        }

        /// <summary>
        /// Sets the target as a Transform object.
        /// </summary>
        /// <param name="targetTransform">The target Transform to follow.</param>
        /// <param name="targetDistance">The distance at which the Character2D is considered to have reached the target.</param>
        /// <param name="newDetermination">Optional. If true, this setting will override the previous one.</param>
        public void SetTarget(Transform targetTransform, float targetDistance, bool newDetermination = false)
        {
            if (_determined == true && newDetermination == false) { return; }

            DisableTarget();

            type = TargetType.Transform;
            _targetTransform = targetTransform;
            targetReachedDistance = targetDistance;
            _isReached = false;
            _determined = newDetermination;

            _calculatePathAtNextPossibility = true;
        }

        /// <summary>
        /// Sets the target as another Character2D object.
        /// </summary>
        /// <param name="otherCharacter">The other Character2D target to follow.</param>
        /// <param name="targetDistance">The distance at which the Character2D is considered to have reached the target.</param>
        /// <param name="newDetermination">Optional. If true, this setting will override the previous one.</param>
        public void SetTarget(Character2D otherCharacter, float targetDistance, bool newDetermination = false)
        {
            if (_determined == true && newDetermination == false) { return; }
            _determined = newDetermination;

            DisableTarget();

            if (otherCharacter == null) { return; }

            type = TargetType.Character;
            _otherCharacter = otherCharacter;
            _targetTransform = otherCharacter.transform;
            targetReachedDistance = targetDistance;
            _isReached = false;
            _determined = newDetermination;

            // Subscribe to the stateChanged event of the otherCharacter to handle its state changes.
            _otherCharacter.stateChanged += Character_StateChanged;

            _calculatePathAtNextPossibility = true;
        }

        /// <summary>
        /// Disables the current target and clears any target-related data.
        /// </summary>
        public void DisableTarget()
        {
            type = TargetType.None;

            if (_otherCharacter != null)
            {
                // Unsubscribe from the stateChanged event to avoid memory leaks.
                _otherCharacter.stateChanged -= Character_StateChanged;
                _otherCharacter = null;
            }
            _targetTransform = null;

            // Reset path parameters.
            _hasPath = false;
            _calculatePathAtNextPossibility = false;

            _isReached = true;

            _determined = false;
        }

        /// <summary>
        /// Sets the pathfinding field to be used for calculating the path to the target.
        /// </summary>
        /// <param name="field">The pathfinding field implementing the IPathField interface.</param>
        public void SetPathField(IPathField field)
        {
            // Disables the current target and clears any target-related data before setting a new path field.
            DisableTarget();

            _pathField = field;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the performance ticker and triggers path calculation if needed.
        /// </summary>
        private void PerformanceTicker()
        {
            _ticker++;
            if (_ticker >= _maxTicker)
            {
                _ticker = 0;

                if (_calculatePathAtNextPossibility)
                {
                    _calculatePathAtNextPossibility = false;

                    // Calculate the path to the target.
                    CalculatePath();
                }
            }
        }

        /// <summary>
        /// Marks the target as reached and handles target event invocation.
        /// </summary>
        private void ReachTarget()
        {
            _isReached = true;

            // Target reached, disable the target if it's a position or a transform.
            if (type == TargetType.Position || type == TargetType.Transform) { DisableTarget(); }

            // Invoke the target event to notify listeners that the target has been reached.
            if (targetEvent != null) { targetEvent(TargetEventType.TargetReached); }
        }

        /// <summary>
        /// Calculates the path to the target using the assigned pathfinding field.
        /// </summary>
        private void CalculatePath()
        {
            // Check if a pathfinding field exists and the protagonist can move.
            if (_pathField != null && _protagonist.movementSpeed > 0)
            {
                // Get the path to the target using the pathfinding field.
                _pathField.GetPath(_protagonistTransform.position, GetFinalTargetPosition(), _path);

                // Check if a valid path was found.
                _hasPath = _path.isValid;
                if (!_hasPath)
                {
                    // If the target is a position, mark it as reached as it's not reachable.
                    if (type == TargetType.Position) { ReachTarget(); }
                }
            }
        }

        /// <summary>
        /// Moves to the next node in the calculated path.
        /// </summary>
        private void NextNodeInPath()
        {
            if (_hasPath && _path.isValid)
            {
                _path.NextPosition();
            }

            // If the path is invalid or the last node has been reached, mark the path as finished.
            if (!_path.isValid || _path.hasFinished) { _hasPath = false; }
        }

        /// <summary>
        /// Handles the state change of the otherCharacter and disables the target if the character is no longer alive.
        /// </summary>
        /// <param name="character">The otherCharacter whose state changed.</param>
        /// <param name="state">The new state of the otherCharacter.</param>
        private void Character_StateChanged(ICharacter character, CharacterState state)
        {
            // If the otherCharacter is no longer alive, disable the target and invoke the target event to notify listeners.
            if (!character.isAlive)
            {
                DisableTarget();

                if (targetEvent != null) { targetEvent(TargetEventType.CannotReachTarget); }
            }
        }

        #endregion
    }
}
