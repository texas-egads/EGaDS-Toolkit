# Application System User Guide

## Table of Contents
- [Overview](#overview)
- [Classes](#classes)
  - [ApplicationInfo](#applicationinfo)
- [Usage Examples](#usage-examples)
  - [Initialize an App](#initialize-an-app)

## Overview
The `ApplicationInfo` class contains information about the application's settings and platform-specific configurations. It is used to determine various input capabilities of the device, such as whether it has two mouse buttons, supports touch input, or uses a gamepad. This guide will provide an overview of the class and explain its public properties, constructors, and methods.

## Classes
### ApplicationInfo
This class represents the application information and settings. It contains the following public properties and methods:

#### Public Properties
- `hasTwoMouseButtons`: Indicates whether the device has two mouse buttons (e.g., right-click and left-click).
- `useTwoMouseButtons`: Indicates whether the application uses two mouse buttons for input.
- `hasTouch`: Indicates whether the device supports touch input.
- `hasMouse`: Indicates whether the device supports mouse input.

#### Gamepad Setup
- `usesGamepad`: Checks if the application uses a gamepad for input. This property returns `true` if any joystick is connected.

#### Constructor
- `ApplicationInfo()`: Initializes a new instance of the `ApplicationInfo` class and sets the platform-specific settings. It also loads input settings from `PlayerPrefs`, and in Unity editor, it may override some settings based on debug flags.

#### Methods
- `SetTwoMouseButtonMode(bool setting)`: Sets the application's two mouse button mode based on the provided boolean `setting`. It saves the setting to `PlayerPrefs` for persistent storage.
- `SetPlatformSettings()`: Sets the platform-specific settings based on the current application platform. It sets properties like `hasTwoMouseButtons`, `hasTouch`, and `hasMouse` accordingly.

## Usage Examples
The following examples demonstrate how to set up the `ApplicationInfo` class for different application platforms. Depending on the platform, the class will configure the input settings accordingly.

### Initialize an App
```csharp
using UnityEngine;
using egads.system.application;

public class AppInitializer : MonoBehaviour
{
    private void Start()
    {
        ApplicationInfo appInfo = new ApplicationInfo();
        // Now appInfo has the correct platform-specific input settings for whatever platform.
    }
}
```
