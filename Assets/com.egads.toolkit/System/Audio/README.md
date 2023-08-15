
# Audio System User Guide

  
The Audio System provides a set of classes that allow you to handle audio playback and effects in your Unity projects. Each class is designed to serve a specific purpose, from playing random sounds with configurable pause intervals to crossfading between multiple audio clips for smoother transitions.
  

## Table of Contents

- [Classes](#classes)
    - [AudioManager](#audiomanager)
    - [RandomSound](#randomsound)
    - [RandomSoundVolume](#randomsoundvolume)
    - [SoundCrossFading](#soundcrossfading)
    - [SteppingSound](#steppingsound)

- [Usage Examples](#usage-examples)
    - [AudioManager Usage](#audiomanager-usage)
    - [RandomSound Usage](#randomsound-usage)
    - [RandomSoundVolume Usage](#randomsoundvolume-usage)
    - [SoundCrossFading Usage](#soundcrossfading-usage)
    - [SteppingSound Usage](#steppingsound-usage)

## Classes

### AudioManager
Manages audio playback in the game and provides various methods for playing audio clips.
Public Methods
-   `PlayButtonSound()`: Play the button interaction sound.
-   `PlayRandomSound(AudioClip[] clips)`: Play a random sound from the given array of audio clips.
-   `PlayOnce(AudioClip clip, float volume = 1.0f)`: Plays an audio clip if it was not played recently within a specified duration, with optional volume control.
-   `Play(AudioClip clip, float volume = 1.0f)`: Play an audio clip with a given volume.
-   `PlayWithVariation(AudioClip clip, float volume = 1.0f)`: Play an audio clip with pitch and volume variations.

### RandomSound
Plays random sounds with configurable pause intervals.
Public Properties
-  `source`: The AudioSource used to play the random sounds.
-  `pauseIntervalMin`: The minimum interval time between sounds.
-  `pauseIntervalMax`: The maximum interval time between sounds.

### RandomSoundVolume
Plays a random sound with an adjustable volume fading effect.
Public Properties
-  `source`: The AudioSource used to play the random sound.
-  `intervalMin`: The minimum interval time between volume changes.
-  `intervalMax`: The maximum interval time between volume changes.
-  `fadeInFadeOut`: If true, the sound will fade in and out between volume changes.

### SoundCrossFading
Crossfades between multiple audio clips for smoother transitions.
Public Properties
-  `clips`: The list of audio clips to crossfade between.
-  `firstSource`: The first AudioSource used for playback.
-  `secondSource`: The second AudioSource used for playback.

### SteppingSound
Plays stepping sounds when the associated Character2D is moving.
Public Properties
-  `source`: The AudioSource used to play the stepping sounds.

## Usage Examples
Below are some usage instructions for each class:

### AudioManager Usage
-   Attach the `AudioManager` script to an empty `GameObject` in your scene
-   Make sure the `GameObject` has an `AudioSource` component attached
-   Configure the public fields in the `AudioManager` script, such as `_buttonSound`, according to your audio assets

Here are some examples of how to use the `AudioManager` class:
```csharp
// Play a button interaction sound
AudioManager.Instance.PlayButtonSound();

// Play a random sound from an array of clips
AudioClip[] randomClips = { clip1, clip2, clip3 };
AudioManager.Instance.PlayRandomSound(randomClips);

// Play an audio clip only if it wasn't played recently
AudioManager.Instance.PlayOnce(specialClip);

// Play an audio clip with a specific volume
AudioManager.Instance.Play(someClip, 0.5f);

// Play an audio clip with pitch and volume variations
AudioManager.Instance.PlayWithVariation(effectsClip);
```
Remember to replace `clip1`, `clip2`, `clip3`, `specialClip`, and `effectsClip` with actual `AudioClip` references.

### RandomSound Usage
- Attach the `RandomSound` script to an empty `GameObject` with an `AudioSource` component
- Configure the `pauseIntervalMin` and `pauseIntervalMax` properties in the inspector

### RandomSoundVolume Usage
- Attach the `RandomSoundVolume` script to an empty `GameObject` with an `AudioSource` component
- Configure the `intervalMin` and `intervalMax` properties in the inspector

### SoundCrossFading Usage
- Attach the `SoundCrossFading` script to an empty `GameObject` with two `AudioSource` components
- Add audio clips to the clips list in the inspector

### SteppingSound Usage

- Attach the `SteppingSound` script to a character `GameObject` with an `AudioSource` component
- The associated `Character2D` script should handle the movement logic