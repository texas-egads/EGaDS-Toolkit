using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using egads.tools.extensions;
using egads.tools.objects;
using egads.tools.text;
using egads.system.audio;
using egads.system.input;
using egads.system.application;
using egads.system.pathFinding;
using egads.system.camera;
using egads.system.UI;
using egads.system.timer;
using System.Collections.Generic;
using System;

namespace egads.system.gameManagement
{
    /// <summary>
    /// The central class responsible for managing game states, components, and various functionalities.
    /// </summary>
    public class MainBase : MonoBehaviour
    {
        #region Delegates & Events

        /// <summary>
        /// Delegate for handling game state changes.
        /// </summary>
        /// <param name="newState">The new game state.</param>
        public delegate void GameStateChangeHandler(GameState newState);
        public event GameStateChangeHandler gameStateChanged;

        #endregion

        #region State Machine Properties

        /// <summary>
        /// Represents the game state and manages the state machine.
        /// </summary>
        protected Stack<GameState> _stateMachine = new Stack<GameState>(new[] { GameState.Running });

        /// <summary>
        /// Gets or sets the current game state.
        /// </summary>
        public GameState state
        {
            get { return _stateMachine.Peek(); }
            protected set
            {
                // If paused, push on top of the state machine
                if (value == GameState.Paused) { _stateMachine.Push(GameState.Paused); }
                else
                {
                    // Clear the state machine and set the new state
                    _stateMachine.Clear();
                    _stateMachine.Push(value);
                }

                // Notify listeners about the state change
                if (gameStateChanged != null) { gameStateChanged(value); }
            }
        }

        #endregion

        #region Game State Properties

        /// <summary>
        /// Checks if the current scene is a menu level.
        /// </summary>
        public static bool isMenuLevel => SceneManager.GetActiveScene().name.ToLower().Contains("menu");

        /// <summary>
        /// Checks if the game is currently running.
        /// </summary>
        public static bool isRunning { get { return Instance.state == GameState.Running; } }

        /// <summary>
        /// Checks if the game is currently running or in a sequence.
        /// </summary>
        public static bool isRunningOrInSequence { get { return Instance.state == GameState.Running || Instance.state == GameState.Sequence; } }

        /// <summary>
        /// Checks if the game is currently paused.
        /// </summary>
        public static bool isPaused { get { return Instance.state == GameState.Paused; } }

        #endregion

        #region Singleton

        /// <summary>
        /// The singleton instance of the MainBase class.
        /// </summary>
        public static MainBase Instance;

        #endregion

        #region Game Components

        // BaseNavigationInput component for handling navigation input.
        [HideInInspector]
        public BaseNavigationInput baseNavigationInput;

        /// <summary>
        /// Gets the instance of the BaseNavigationInput component.
        /// </summary>
        public static BaseNavigationInput BaseNavigationInput => Instance.baseNavigationInput;

        // Application information data.
        [HideInInspector]
        public AppInfo applicationInfo;

        /// <summary>
        /// Gets the instance of the AppInfo data.
        /// </summary>
        public static AppInfo Info => Instance.applicationInfo;

        // BaseAudioManager component for managing audio.
        [HideInInspector]
        public AudioManager baseAudioManager;

        /// <summary>
        /// Gets the instance of the BaseAudioManager component.
        /// </summary>
        public static AudioManager BaseAudioManager => Instance.baseAudioManager;

        // Rect representing level bounds.
        [HideInInspector]
        public Rect levelBounds;

        /// <summary>
        /// Gets the instance of the level bounds as a Rect.
        /// </summary>
        public static Rect LevelBounds => Instance.levelBounds;

        // LevelGrid component for managing level layout and objects.
        [HideInInspector]
        public LevelGrid levelGrid = null;

        /// <summary>
        /// Gets the instance of the LevelGrid component.
        /// </summary>
        public static LevelGrid LevelGrid => Instance.levelGrid;

        /// <summary>
        /// Gets the level grid for the specified position.
        /// </summary>
        /// <param name="pos">The position for which to get the LevelGrid.</param>
        /// <returns>The LevelGrid instance.</returns>
        public virtual LevelGrid GetLevelGrid(Vector2 pos) => LevelGrid;

        // CameraShake component for handling camera shaking effects.
        [HideInInspector]
        public CameraShake screenShake;

        /// <summary>
        /// Gets the instance of the CameraShake component.
        /// </summary>
        public static CameraShake ScreenShake => Instance.screenShake;

        // Messenger component for sending and receiving messages.
        [HideInInspector]
        public Messenger messenger;

        /// <summary>
        /// Gets the instance of the Messenger component.
        /// </summary>
        public static Messenger Messenger => Instance.messenger;

        // InputController for handling user input.
        public IInputController inputController = null;

        /// <summary>
        /// Gets the instance of the IInputController.
        /// </summary>
        public static IInputController InputController => Instance.inputController;

        // GamepadInput component for handling gamepad input.
        public IGamepadInput gamepadInput = null;

        /// <summary>
        /// Gets the instance of the IGamepadInput component.
        /// </summary>
        public static IGamepadInput GamepadInput => Instance.gamepadInput;

        // GameStateData for storing game state information.
        public GameStateData gameStateData = new GameStateData();

        /// <summary>
        /// Gets the instance of the GameStateData component.
        /// </summary>
        public static GameStateData GameStateData => Instance.gameStateData;

        // PrefabPool for managing object pooling.
        public PrefabPool prefabPool = new PrefabPool();

        /// <summary>
        /// Gets the instance of the PrefabPool component.
        /// </summary>
        public static PrefabPool PrefabPool => Instance.prefabPool;

        // Timers for managing various time-based events.
        public Timers timers = new Timers();

        /// <summary>
        /// Gets the instance of the Timers component.
        /// </summary>
        public static Timers Timers => Instance.timers;

        #endregion

        #region Debug Properties

        /// <summary>
        /// Indicates whether touch input is in debug mode.
        /// </summary>
        public bool debugIsTouch;

        /// <summary>
        /// Indicates whether audio is disabled for debugging.
        /// </summary>
        public bool debugDisableAudio;

        #endregion

        #region Private Properties

        /// <summary>
        /// Timer used for loading operations.
        /// </summary>
        private Timer _loadingTimer = null;

        /// <summary>
        /// Flag that is set when the application loses focus and switches automatically to pause mode.
        /// </summary>
        private bool _automaticallyPaused = false;

        /// <summary>
        /// The previous game state before loading a new level.
        /// </summary>
        GameState? _stateBeforeLoading = null;

        /// <summary>
        /// AsyncOperation for tracking loading progress.
        /// </summary>
        private AsyncOperation loadingOperation = null;

        #endregion

        #region Unity Methods

        /// <summary>
        /// Called when the object is initialized.
        /// Registers the sceneLoaded event handler.
        /// </summary>
        protected void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        /// <summary>
        /// Called when the object is started.
        /// Loads level data and initializes the level.
        /// </summary>
        protected virtual void Start()
        {
            LoadLevelData();
            InitLevel();
        }

        /// <summary>
        /// Called every frame.
        /// Manages the game update loop, handling loading, timers, and updates.
        /// </summary>
        protected virtual void Update()
        {
            // Handle loading state
            if (state == GameState.Loading)
            {
                if (_loadingTimer != null) { _loadingTimer.Update(); }

                // Check loading operation progress
                if (loadingOperation != null)
                {
                    if (loadingOperation.isDone) { loadingOperation = null; }
                }

                // Check if loading has completed
                if (loadingOperation == null)
                {
                    loadingOperation = null;

                    if (_loadingTimer != null)
                    {
                        if (_loadingTimer.hasEnded) { _loadingTimer = null; }
                    }
                    else { InitLevel(); } // Initialize the level after loading
                }
            }
            else { timers.Update(); } // Update timers during normal gameplay
        }

        /// <summary>
        /// Called when a new scene is loaded.
        /// Clears object pool, timers, and reloads level data.
        /// </summary>
        /// <param name="scene">The loaded scene.</param>
        /// <param name="mode">The load scene mode.</param>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Clear object pool and timers on scene load
            prefabPool.Reset();
            timers.Clear();

            LoadLevelData(); // Load level-specific data
            if (_loadingTimer == null) { InitLevel(); } // Initialize the level if not loading

            // Perform memory garbage collection (only for Mono)
            System.GC.Collect();
        }

        /// <summary>
        /// Called when the application's pause status changes.
        /// Automatically pauses or resumes the game based on pause status.
        /// </summary>
        /// <param name="pauseStatus">The pause status of the application.</param>
        protected virtual void OnApplicationPause(bool pauseStatus)
        {
            // Automatically pause or resume based on pause status
            if (pauseStatus && (state == GameState.Running || state == GameState.Sequence))
            {
                Pause();
                _automaticallyPaused = true;
            }

            if (!pauseStatus && state == GameState.Paused && _automaticallyPaused) { Resume(); }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Resumes the game from a paused state.
        /// </summary>
        public virtual void Resume()
        {
            _automaticallyPaused = false;
            Time.timeScale = 1.0f;

            // Pop paused state from state machine
            if (state == GameState.Paused)
            {
                _stateMachine.Pop();
                if (gameStateChanged != null) { gameStateChanged(state); }
            }
        }

        /// <summary>
        /// Pauses the game, changing the state to Paused.
        /// </summary>
        public virtual void Pause()
        {
            state = GameState.Paused;
        }

        /// <summary>
        /// Sets a timer for loading operations.
        /// </summary>
        public void SetLoadingTimer()
        {
            _loadingTimer = new Timer(0.1f);
        }

        /// <summary>
        /// Starts a sequence, changing the state to Sequence.
        /// </summary>
        public void StartSequence()
        {
            state = GameState.Sequence;
        }

        /// <summary>
        /// Ends a sequence and returns to the Running state.
        /// </summary>
        public void EndSequence()
        {
            if (state != GameState.Loading) { state = GameState.Running; }

            // Check if sequence ended right after loading new level
            if (_stateBeforeLoading.HasValue && _stateBeforeLoading.Value == GameState.Sequence) { _stateBeforeLoading = null; }
        }

        /// <summary>
        /// Quits the application.
        /// </summary>
        public void Quit()
        {
            Application.Quit();
        }

        /// <summary>
        /// Gets a prefab of the specified type.
        /// </summary>
        /// <param name="type">The type of the prefab to get.</param>
        /// <returns>The prefab object or null if not found.</returns>
        public virtual object GetPrefab(Type type)
        {
            return null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes the singleton instance and necessary components.
        /// </summary>
        /// <returns>Returns false if the instance is already initialized.</returns>
        protected bool Init()
        {
            if (Instance != null) { return false; }

            Instance = this;

            // Initialize TextManagerInit if available
            TextManagerInit textManagerInit = GetComponentInChildren<TextManagerInit>();
            if (textManagerInit != null) { textManagerInit.Init(); }

            baseAudioManager = GetComponent<AudioManager>();
            baseNavigationInput = GetComponent<BaseNavigationInput>();
            messenger = GetComponent<Messenger>();
            applicationInfo = new AppInfo();

            gamepadInput = transform.GetInterface<IGamepadInput>();
            inputController = transform.GetInterface<IInputController>();

            // Object shall persist through all levels
            DontDestroyOnLoad(gameObject);

            return true;
        }

        /// <summary>
        /// Loads level-specific data and references in the scene.
        /// </summary>
        protected virtual void LoadLevelData()
        {
            screenShake = FindObjectOfType<CameraShake>();
            levelGrid = FindObjectOfType<LevelGrid>();
        }

        /// <summary>
        /// Initializes the gameplay for the current level.
        /// Sets the state based on the current level type.
        /// </summary>
        protected virtual void InitLevel()
        {
            if (isMenuLevel) { state = GameState.Menu; }
            else
            {
                if (_stateBeforeLoading != null)
                {
                    state = _stateBeforeLoading.Value;
                    _stateBeforeLoading = null;
                }
                else { state = GameState.Running; }
            }
        }

        /// <summary>
        /// Loads a level asynchronously by name.
        /// </summary>
        /// <param name="levelName">The name of the level to load.</param>
        /// <returns>An IEnumerator for handling the asynchronous loading process.</returns>
        protected IEnumerator LoadLevel(string levelName)
        {
            if (state == GameState.Sequence) { _stateBeforeLoading = state; }

            state = GameState.Loading;
            SetLoadingTimer();
            yield return null;
            loadingOperation = SceneManager.LoadSceneAsync(levelName);
        }

        /// <summary>
        /// Loads a level asynchronously by index.
        /// </summary>
        /// <param name="levelIndex">The index of the level to load.</param>
        /// <returns>An IEnumerator for handling the asynchronous loading process.</returns>
        protected IEnumerator LoadLevel(int levelIndex)
        {
            state = GameState.Loading;
            SetLoadingTimer();
            yield return null;
            loadingOperation = SceneManager.LoadSceneAsync(levelIndex);
        }

        #endregion
    }
}
