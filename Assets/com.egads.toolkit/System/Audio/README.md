# Audio System User Guide

The Audio System is a collection of classes designed to handle various audio functionalities in Unity. This guide provides an overview of the system and explains the usage of each class. The Audio System includes classes such as RandomSound, RandomSoundVolume, SoundCrossFading, and SteppingSound.

## Table of Contents
- [Overview](#overview)
- [Classes](#classes)
  - [RandomSound](#randomsound)
  - [RandomSoundVolume](#randomsoundvolume)
  - [SoundCrossFading](#soundcrossfading)
  - [SteppingSound](#steppingsound)
- [Usage Examples](#usage-examples)
  - [RandomSound Usage](#randomsound-usage)
  - [RandomSoundVolume Usage](#randomsoundvolume-usage)
  - [SoundCrossFading Usage](#soundcrossfading-usage)
  - [SteppingSound Usage](#steppingsound-usage)

## Overview

The Audio System provides a set of classes that allow you to handle audio playback and effects in your Unity projects. Each class is designed to serve a specific purpose, from playing random sounds with configurable pause intervals to crossfading between multiple audio clips for smoother transitions.

## Classes

### RandomSound

Plays random sounds with configurable pause intervals.

Public Properties
- `source`: The AudioSource used to play the random sounds.
- `pauseIntervalMin`: The minimum interval time between sounds.
- `pauseIntervalMax`: The maximum interval time between sounds.

### RandomSoundVolume

Plays a random sound with an adjustable volume fading effect.

Public Properties
- `source`: The AudioSource used to play the random sound.
- `intervalMin`: The minimum interval time between volume changes.
- `intervalMax`: The maximum interval time between volume changes.
- `fadeInFadeOut`: If true, the sound will fade in and out between volume changes.

### SoundCrossFading

Crossfades between multiple audio clips for smoother transitions.

Public Properties
- `clips`: The list of audio clips to crossfade between.
- `firstSource`: The first AudioSource used for playback.
- `secondSource`: The second AudioSource used for playback.

### SteppingSound

Plays stepping sounds when the associated Character2D is moving.

Public Properties
- `source`: The AudioSource used to play the stepping sounds.

## Usage Examples

Below are some usage instructions for each class:

### RandomSound
- Attach the `RandomSound` script to an empty `GameObject` with an `AudioSource` component
- Configure the `pauseIntervalMin` and `pauseIntervalMax` properties in the inspector

### RandomSoundVolume
- Attach the `RandomSoundVolume` script to an empty `GameObject` with an `AudioSource` component
- Configure the `intervalMin` and `intervalMax` properties in the inspector

### SoundCrossFading
- Attach the `SoundCrossFading` script to an empty `GameObject` with two `AudioSource` components
- Add audio clips to the clips list in the inspector

### SteppingSound
- Attach the `SteppingSound` script to a character `GameObject` with an `AudioSource` component
- The associated `Character2D` script should handle the movement logic
