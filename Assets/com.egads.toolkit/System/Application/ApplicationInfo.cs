using UnityEngine;
using egads.system.gameManagement;

namespace egads.system.application
{
    public class ApplicationInfo
    {
        #region Public Properties

        public bool hasTwoMouseButtons = false;
        public bool useTwoMouseButtons = false;
        public bool hasTouch = false;
        public bool hasMouse = false;

        #endregion

        #region Gamepad Setup
        public bool usesGamepad
        {
            get
            {
                #if !UNITY_WP8
                if (Input.GetJoystickNames().Length > 0) { return true; }
				if (isOuyaSupportedHardware) { return true; }
                #endif

                return false;
            }
        }

        #endregion

        #region Ouya Setup

        private bool _isOuya = false;
        public bool isOuyaSupportedHardware
        {
            get { return _isOuya; }
            set
            {
                _isOuya = value;
                if (_isOuya) { hasTouch = false; }
            }
        }

        public bool canPostOnSocialNetworks => (!isOuyaSupportedHardware && !(Application.platform == RuntimePlatform.WebGLPlayer));

        #endregion

        #region Contructor

        public ApplicationInfo()
        {
            SetPlatformSettings();

            useTwoMouseButtons = hasTwoMouseButtons;

            // Load input setting
            if (PlayerPrefs.HasKey("INPUT_USETWOMOUSEBUTTONS"))
            {
                int useTwoMouseButtonsSetting = PlayerPrefs.GetInt("INPUT_USETWOMOUSEBUTTONS");
                useTwoMouseButtons = useTwoMouseButtonsSetting == 1;
            }

            // Debug: override settings
            if (Application.isEditor && MainBase.Instance.debugIsTouch)
            {
                hasTouch = true;
                hasTwoMouseButtons = false;
                useTwoMouseButtons = false;
            }

            // Debug: override settings
            if (Application.isEditor && MainBase.Instance.debugDisableAudio)
            {
                AudioListener.volume = 0;
            }
        }

        #endregion

        #region Two Mouse Button Mode

        public void SetTwoMouseButtonMode(bool setting)
        {
            useTwoMouseButtons = setting;

            // Save input setting to File
            if (setting)
                PlayerPrefs.SetInt("INPUT_USETWOMOUSEBUTTONS", 1);
            else
                PlayerPrefs.SetInt("INPUT_USETWOMOUSEBUTTONS", 0);
        }

        #endregion

        #region Platform Settings

        private void SetPlatformSettings()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    hasTwoMouseButtons = false;
                    hasTouch = true;
                    if (isOuyaSupportedHardware)
                        hasTouch = false;
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