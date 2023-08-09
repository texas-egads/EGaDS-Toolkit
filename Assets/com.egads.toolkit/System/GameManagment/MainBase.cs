using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using egads.tools.extensions;
using egads.tools.objects;
using egads.tools.text;
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
    public class MainBase : MonoBehaviour
    {
        #region Delegates & Events

        public delegate void GameStateChangeHandler(GameState newState);
        public event GameStateChangeHandler gameStateChanged;

        #endregion

        #region State Machine Properties

        protected Stack<GameState> _stateMachine = new Stack<GameState>(new[] { GameState.Running });
        public GameState state
        {
            get { return _stateMachine.Peek(); }
            protected set
            {
                // If paused then push on top
                if (value == GameState.Paused) { _stateMachine.Push(GameState.Paused); }
                else
                {
                    _stateMachine.Clear();
                    _stateMachine.Push(value);
                }

                if (gameStateChanged != null) { gameStateChanged(value); }
            }
        }

        #endregion

        #region Game State Properties

        public static bool isMenuLevel => SceneManager.GetActiveScene().name.ToLower().Contains("menu");
		public static bool isRunning { get { return Instance.state == GameState.Running; } }
		public static bool isRunningOrInSequence { get { return Instance.state == GameState.Running || Instance.state == GameState.Sequence; } }
		public static bool isPaused { get { return Instance.state == GameState.Paused; } }

        #endregion

        #region Singleton

        public static MainBase Instance;

        #endregion

        #region Game Components

        [HideInInspector]
		public BaseNavigationInput baseNavigationInput;
		public static BaseNavigationInput BaseNavigationInput => Instance.baseNavigationInput;

		[HideInInspector]
        public AppInfo applicationInfo;
		public static AppInfo Info => Instance.applicationInfo;

        [HideInInspector]
        public BaseAudioManager baseAudioManager;
		public static BaseAudioManager BaseAudioManager => Instance.baseAudioManager;

        [HideInInspector]
        public Rect levelBounds;
		public static Rect LevelBounds => Instance.levelBounds;

        [HideInInspector]
        public LevelGrid levelGrid = null;
		public static LevelGrid LevelGrid => Instance.levelGrid;
        public virtual LevelGrid GetLevelGrid(Vector2 pos) => LevelGrid;

		[HideInInspector]
		public CameraShake screenShake;
		public static CameraShake ScreenShake => Instance.screenShake;

		[HideInInspector]
		public Messenger messenger;
		public static Messenger Messenger => Instance.messenger;

		public IInputController inputController = null;
		public static IInputController InputController => Instance.inputController;

		public IGamepadInput gamepadInput = null;
		public static IGamepadInput GamepadInput => Instance.gamepadInput;

        public GameStateData gameStateData = new GameStateData();
		public static GameStateData GameStateData => Instance.gameStateData;

		public PrefabPool prefabPool = new PrefabPool();
		public static PrefabPool PrefabPool => Instance.prefabPool;

		public Timers timers = new Timers();
		public static Timers Timers => Instance.timers;

        #endregion

        #region Debug Properties

        public bool debugIsTouch;
        public bool debugDisableAudio;

        #endregion

        #region Private Properties

        private Timer _loadingTimer = null;

        // Flag thas is set when the Application looses Focus and switches automatically to pause mode
        private bool _automaticallyPaused = false;

		GameState? _stateBeforeLoading = null;

		private AsyncOperation loadingOperation = null;

        #endregion

        #region Unity Methods

        protected void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        protected virtual void Start()
        {
            LoadLevelData();

            InitLevel();
        }

        protected virtual void Update()
        {
            if (state == GameState.Loading)
            {
                if (_loadingTimer != null) { _loadingTimer.Update(); }

				if (loadingOperation != null)
				{
					if (loadingOperation.isDone) { loadingOperation = null; }	
				}

                if (loadingOperation == null)
                {
					loadingOperation = null;

                    if (_loadingTimer != null)
                    {
                        if (_loadingTimer.hasEnded) { _loadingTimer = null; }
                    }
                    else { InitLevel(); }
                }
            }
			else { timers.Update(); }
		}

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
			// Clear object pool
			prefabPool.Reset();
			timers.Clear();

            LoadLevelData();

            if (_loadingTimer == null) { InitLevel(); }

			// Memory garbage collection (only Mono)
			System.GC.Collect();
        }

        protected virtual void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && (state == GameState.Running || state == GameState.Sequence))
            {
                Pause();
                _automaticallyPaused = true;
            }

            if (!pauseStatus && state == GameState.Paused && _automaticallyPaused) { Resume(); }
        }

        #endregion

        #region Public Methods

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

        public virtual void Pause()
        {
            state = GameState.Paused;
        }

        public void SetLoadingTimer()
        {
            _loadingTimer = new Timer(0.1f);
        }

        public void StartSequence()
        {
            state = GameState.Sequence;
        }

        public void EndSequence()
        {
			if (state != GameState.Loading) { state = GameState.Running; }

			// Check if sequence ended right after loading new level
			if (_stateBeforeLoading.HasValue && _stateBeforeLoading.Value == GameState.Sequence) { _stateBeforeLoading = null; }
        }

        public void Quit()
        {
			Application.Quit();
        }

		public virtual object GetPrefab(Type type)
		{
			return null;
		}

        #endregion

        #region Private Methods

        protected bool Init()
        {
            if (Instance != null) { return false; }

            Instance = this;

			TextManagerInit textManagerInit = GetComponentInChildren<TextManagerInit>();
			if (textManagerInit != null) { textManagerInit.Init(); }

            baseAudioManager = GetComponent<BaseAudioManager>();
			baseNavigationInput = GetComponent<BaseNavigationInput>();
			messenger = GetComponent<Messenger>();
			applicationInfo = new AppInfo();

			gamepadInput = transform.GetInterface<IGamepadInput>();
			inputController = transform.GetInterface<IInputController>();

			// Object shall persist through all levels
			DontDestroyOnLoad(gameObject);

            return true;
        }

		// use this for finding references in the scene
        protected virtual void LoadLevelData()
        {
			screenShake = FindObjectOfType<CameraShake>();
			levelGrid = FindObjectOfType<LevelGrid>();
        }

		// use this for initialising gameplay
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

        protected IEnumerator LoadLevel(string levelName)
        {
			if (state == GameState.Sequence) { _stateBeforeLoading = state; }

            state = GameState.Loading;
            SetLoadingTimer();
			yield return null;
			loadingOperation = SceneManager.LoadSceneAsync(levelName);
        }

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