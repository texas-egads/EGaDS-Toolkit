using UnityEngine;
using egads.tools.utils;
using egads.system.pathFinding;

namespace egads.system.actors
{
    /// <summary>
    /// Represents a target for an Actor2D to follow, such as a position, another Actor, or a Transform.
    /// </summary>
    public class ActorTarget
    {
        #region Constants

        /// <summary>
        /// The squared distance threshold for considering the Actor to have reached a path node.
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
            /// The target is another Actor2D.
            /// </summary>
            Actor,

            /// <summary>
            /// There is no target.
            /// </summary>
            None
        }

        /// <summary>
        /// The type of event triggered when the Actor reaches or cannot reach the target.
        /// </summary>
        public enum TargetEventType
        {
            /// <summary>
            /// The Actor successfully reached the target.
            /// </summary>
            TargetReached,

            /// <summary>
            /// The Actor cannot reach the target, usually when the target Actor is no longer alive.
            /// </summary>
            CannotReachTarget
        }

        /// <summary>
        /// The target type assigned to this ActorTarget.
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
        /// Event triggered when the Actor reaches or cannot reach the target.
        /// </summary>
        public event TargetEventDelegate targetEvent;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether the Actor has reached its target.
        /// </summary>
        public bool isReached => _isReached;

        /// <summary>
        /// Gets a value indicating whether the Actor has a target assigned.
        /// </summary>
        public bool hasTarget => (type != TargetType.None);

        /// <summary>
        /// Gets a value indicating whether the Actor's target is another Actor.
        /// </summary>
        public bool hasActorTarget => type == TargetType.Actor;

        #endregion

        #region Private Properties

        // Flag indicating if the Actor has reached its target.
        private bool _isReached = true;

        // The Transform of the Actor2D instance who is following the target.
        private Transform _protagonistTransform;

        // The Actor2D instance who is following the target.
        private Actor2D _protagonist;

        // The target position to follow (used when the target type is Position).
        private Vector2 _targetPosition;

        // The target Transform to follow (used when the target type is Transform or Actor).
        private Transform _targetTransform;

        // The other Actor2D target (used when the target type is Actor).
        private Actor2D _otherActor;

        /// <summary>
        /// Gets the other Actor2D target when the target type is Actor.
        /// </summary>
        public Actor2D otherActor => _otherActor;

        // Saving squared value of target reached distance for performance reasons when comparing vector length.
        private float _targetReachedDistanceSquared;

        // The target reached distance (squared), used to determine if the target is reached.
        private float _targetReachedDistance;

        /// <summary>
        /// Gets or sets the distance at which the Actor2D is considered to have reached the target.
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
        /// Initializes a new instance of the ActorTarget class.
        /// </summary>
        /// <param name="protagonist">The Actor2D instance to be controlled by this target.</param>
        /// <param name="protagonistTransform">The Transform component of the Actor2D instance.</param>
        public ActorTarget(Actor2D protagonist, Transform protagonistTransform)
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
        /// Updates the ActorTarget, making the Actor2D follow the target if applicable.
        /// </summary>
        public void Update()
        {
            // Make sure we don't follow a null target Actor.
            if (type == TargetType.Actor && _otherActor == null)
            {
                DisableTarget();
                return;
            }

            // Performance heavy calculations only every few frames.
            if (!_isReached) { PerformanceTicker(); }

            if (type != TargetType.None)
            {
                Vector2 targetPos = GetFinalTargetPosition();

                // Check if the Actor2D has reached the target.
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

            if (type == TargetType.Transform || type == TargetType.Actor) { targetPos = _targetTransform.position; }

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
        /// <param name="targetDistance">The distance at which the Actor2D is considered to have reached the target.</param>
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
        /// <param name="targetDistance">The distance at which the Actor2D is considered to have reached the target.</param>
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
        /// Sets the target as another Actor2D object.
        /// </summary>
        /// <param name="otherActor">The other Actor2D target to follow.</param>
        /// <param name="targetDistance">The distance at which the Actor2D is considered to have reached the target.</param>
        /// <param name="newDetermination">Optional. If true, this setting will override the previous one.</param>
        public void SetTarget(Actor2D otherActor, float targetDistance, bool newDetermination = false)
        {
            if (_determined == true && newDetermination == false) { return; }
            _determined = newDetermination;

            DisableTarget();

            if (otherActor == null) { return; }

            type = TargetType.Actor;
            _otherActor = otherActor;
            _targetTransform = otherActor.transform;
            targetReachedDistance = targetDistance;
            _isReached = false;
            _determined = newDetermination;

            // Subscribe to the stateChanged event of the otherActor to handle its state changes.
            _otherActor.stateChanged += Actor_StateChanged;

            _calculatePathAtNextPossibility = true;
        }

        /// <summary>
        /// Disables the current target and clears any target-related data.
        /// </summary>
        public void DisableTarget()
        {
            type = TargetType.None;

            if (_otherActor != null)
            {
                // Unsubscribe from the stateChanged event to avoid memory leaks.
                _otherActor.stateChanged -= Actor_StateChanged;
                _otherActor = null;
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
        /// Handles the state change of the otherActor and disables the target if the actor is no longer alive.
        /// </summary>
        /// <param name="actor">The otherActor whose state changed.</param>
        /// <param name="state">The new state of the otherActor.</param>
        private void Actor_StateChanged(IActor actor, ActorState state)
        {
            // If the otherActor is no longer alive, disable the target and invoke the target event to notify listeners.
            if (!actor.isAlive)
            {
                DisableTarget();

                if (targetEvent != null) { targetEvent(TargetEventType.CannotReachTarget); }
            }
        }

        #endregion
    }
}
