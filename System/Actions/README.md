# Unity Actions System User Guide

The Unity Actions System is a set of interfaces and classes designed to manage and execute actions in a flexible and organized manner. Actions can be performed either in parallel (ActionList) or sequentially (ActionQueue). The system allows you to define custom actions by implementing the provided interfaces.

## Table of Contents

1. [Overview](#overview)
2. [Interfaces](#interfaces)
   - [IAction](#iaction)
   - [IActionQueueElement](#iactionqueueelement)
   - [IEnumeratedAction](#ienumeratedaction)
3. [Classes](#classes)
   - [ActionBase](#actionbase)
   - [ActionList](#actionlist)
   - [ActionQueue](#actionqueue)
4. [Custom Action Example](#custom-action-example)
5. [Usage Examples](#usage-examples)

## Overview

The Actions System provides a simple yet powerful way to manage actions in Unity. It includes two main classes, `ActionList` and `ActionQueue`, which allow you to execute actions either simultaneously or sequentially.

## Interfaces

### IAction

The `IAction` interface represents a basic action that can be executed. It has a single method:

- `void Execute()`: Executes the action.

### IActionQueueElement

The `IActionQueueElement` interface represents an action that lasts for a specific duration and can be added to a chronological queue. It has three methods:

- `void OnStart()`: Called when the action starts. Perform any necessary setup here.
- `void Update()`: Called each frame while the action is ongoing. Implement the main logic of the action here.
- `void OnExit()`: Called when the action is completed or interrupted. Clean up resources or perform final actions here.

### IEnumeratedAction

The `IEnumeratedAction` interface represents an action whose execution process can be divided into multiple steps using an IEnumerator. This allows the action to be executed in multiple frames and supports iteration. It has a single method:

- `IEnumerator Execute()`: Represents the action's execution process as an IEnumerator.

## Classes

### ActionBase

`ActionBase` is a base class that implements the `IActionQueueElement` interface. It provides default implementations for the interface methods, allowing you to create custom actions more easily.

### ActionList

The `ActionList` class represents a list of actions that get executed simultaneously. Actions in the list will run in parallel, regardless of the execution time of individual actions.

Public Properties

- `bool hasContent`: Gets a value indicating whether the action list has any content (actions).

Public Methods

- `void Update()`: Should be called every frame by the parent object to update all the actions in the list.
- `void Add(IActionQueueElement element)`: Adds a new element to the action list.
- `void Clear()`: Calls OnExit on all elements in the current list and clears all entries from the queue.

### ActionQueue

The `ActionQueue` class represents a list of actions that get executed one after another (in a queue). Actions in the queue will run sequentially.

Public Properties

- `bool hasContent`: Gets a value indicating whether the action queue has any content (actions).

Public Methods

- `void Update()`: Should be called every frame by the parent object to update the action queue.
- `void Add(IActionQueueElement element)`: Adds a new element to the back of the action queue.
- `void AddPause(float duration)`: Adds a time duration where nothing happens (a pause) to the action queue.
- `void Clear()`: Calls OnExit on the current element and clears all entries from the action queue.

## Custom Action Example

Here's an example of creating a custom action using the provided interfaces:

```csharp
using UnityEngine;

namespace egads.system.actions
{
    public class CustomAction : IActionQueueElement
    {
        private bool _isCompleted;

        public bool hasEnded => _isCompleted;

        public void OnStart()
        {
            // Perform setup and initialization for the custom action.
            _isCompleted = false;
        }

        public void Update()
        {
            // Implement the main logic of the custom action.
            // For example, move a game object from one position to another.
            // Once the action is completed, set _isCompleted to true.
            // This will signal the ActionQueue to proceed to the next action.
        }

        public void OnExit()
        {
            // Perform cleanup or final actions when the action is completed or interrupted.
        }
    }
}
```

## Usage Examples
### Using ActionList:
```csharp
using UnityEngine;
using egads.system.actions;

public class ExampleActionListUsage : MonoBehaviour
{
    private ActionList actionList;

    private void Start()
    {
        actionList = new ActionList();

        // Add multiple actions to the list
        actionList.Add(new CustomAction());
        actionList.Add(new AnotherCustomAction());
        actionList.Add(new ActionWait(2f)); // Pause for 2 seconds

        // More actions can be added here...

        // Start the action list
        actionList.Update();
    }

    private void Update()
    {
        // Update the action list each frame
        if (actionList.hasContent)
        {
            actionList.Update();
        }
    }

    // Clear the action list when needed
    private void OnDestroy()
    {
        actionList.Clear();
    }
}
```
### Using ActionQueue:
```csharp
using UnityEngine;
using egads.system.actions;

public class ExampleActionQueueUsage : MonoBehaviour
{
    private ActionQueue actionQueue;

    private void Start()
    {
        actionQueue = new ActionQueue();

        // Add actions to the queue
        actionQueue.Add(new CustomAction());
        actionQueue.AddPause(1.5f); // Pause for 1.5 seconds
        actionQueue.Add(new AnotherCustomAction());
        actionQueue.Add(new ActionWait(3f)); // Pause for 3 seconds

        // More actions can be added here...

        // Start the action queue
        actionQueue.Update();
    }

    private void Update()
    {
        // Update the action queue each frame
        if (actionQueue.hasContent)
        {
            actionQueue.Update();
        }
    }

    // Clear the action queue when needed
    private void OnDestroy()
    {
        actionQueue.Clear();
    }
}
```
Remember to replace `CustomAction` and `AnotherCustomAction` with your own custom actions
that implement the `IActionQueueElement` interface. You can now create complex action
sequences in Unity using the Actions System to perform various tasks in your games or
applications!
