using UnityEngine;
using egads.system.gameManagement;

namespace egads.system.application
{
    /// <summary>
    /// Contains information about the application's settings and platform-specific configurations.
    /// </summary>
    public class ApplicationInfo
    {
        #region Public Properties

        /// <summary>
        /// Whether the device has two mouse buttons (e.g., right-click and left-click).
        /// </summary>
        public bool hasTwoMouseButtons = false;

        /// <summary>
        /// Whether the application uses two mouse buttons for input.
        /// </summary>
        public bool useTwoMouseButtons = false;

        /// <summary>
        /// Whether the device supports touch input.
        /// </summary>
        public bool hasTouch = false;

        /// <summary>
        /// Whether the device supports mouse input.
        /// </summary>
        public bool hasMouse = false;

        #endregion

        #region Gamepad Setup

        /// <summary>
        /// Checks if the application uses a gamepad for input.
        /// </summary>
        public bool usesGamepad
        {
            get
            {
#if !UNITY_WP8
                // Check if any joystick is connected.
                if (Input.GetJoystickNames().Length > 0) { return true; }

#endif

                return false;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ApplicationInfo class and sets the platform-specific settings.
        /// </summary>
        public ApplicationInfo()
        {
            SetPlatformSettings();

            // Load input setting from PlayerPrefs.
            if (PlayerPrefs.HasKey("INPUT_USETWOMOUSEBUTTONS"))
            {
                int useTwoMouseButtonsSetting = PlayerPrefs.GetInt("INPUT_USETWOMOUSEBUTTONS");
                useTwoMouseButtons = useTwoMouseButtonsSetting == 1;
            }

            // Debug: Override settings in the Unity editor.
            if (Application.isEditor && MainBase.Instance.debugIsTouch)
            {
                hasTouch = true;
                hasTwoMouseButtons = false;
                useTwoMouseButtons = false;
            }

            // Debug: Disable audio in the Unity editor.
            if (Application.isEditor && MainBase.Instance.debugDisableAudio) { AudioListener.volume = 0; }
        }

        #endregion

        #region Two Mouse Button Mode

        /// <summary>
        /// Sets the application's two mouse button mode.
        /// </summary>
        /// <param name="setting">The value indicating whether to use two mouse buttons.</param>
        public void SetTwoMouseButtonMode(bool setting)
        {
            useTwoMouseButtons = setting;

            // Save input setting to PlayerPrefs.
            if (setting) { PlayerPrefs.SetInt("INPUT_USETWOMOUSEBUTTONS", 1); }
            else { PlayerPrefs.SetInt("INPUT_USETWOMOUSEBUTTONS", 0); }
        }

        #endregion

        #region Platform Settings

        /// <summary>
        /// Sets the platform-specific settings based on the current application platform.
        /// </summary>
        private void SetPlatformSettings()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    hasTwoMouseButtons = false;
                    hasTouch = true;
                    break;
                case RuntimePlatform.IPhonePlayer:
                    hasTwoMouseButtons = false;
                    hasTouch = true;
                    break;
                case RuntimePlatform.LinuxPlayer:
                    hasTwoMouseButtons = true;
                    hasMouse = true;
                    break;
                case RuntimePlatform.WSAPlayerARM:
                    hasTwoMouseButtons = false;
                    hasMouse = true;
                    break;
                case RuntimePlatform.WSAPlayerX64:
                    hasTwoMouseButtons = false;
                    hasMouse = true;
                    break;
                case RuntimePlatform.WSAPlayerX86:
                    hasTwoMouseButtons = false;
                    hasMouse = true;
                    break;
                case RuntimePlatform.OSXEditor:
                    hasTwoMouseButtons = true;
                    hasMouse = true;
                    break;
                case RuntimePlatform.OSXPlayer:
                    hasTwoMouseButtons = true;
                    hasMouse = true;
                    break;
                case RuntimePlatform.WebGLPlayer:
                    hasTwoMouseButtons = true;
                    hasMouse = true;
                    break;
                case RuntimePlatform.WindowsEditor:
                    hasTwoMouseButtons = true;
                    hasMouse = true;
                    break;
                case RuntimePlatform.WindowsPlayer:
                    hasTwoMouseButtons = true;
                    hasMouse = true;
                    break;
                default:
                    break;
            }
        }

        #endregion
    }
}
