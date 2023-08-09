# Actors System User Guide
The Actors System is a collection of interfaces and classes designed to facilitate the management of 2D actors (game objects) in Unity. It provides a set of components and functionalities that can be used to create and control actors with various behaviors such as movement, health, rendering, and interaction.

## Table of Contents
- [Overview](#overview)
- [Interfaces](#interfaces)
  - [IActor](#iactor)
  - [IActorTimedAction](#iactortimedaction)
  - [IAnimationController](#ianimationcontroller)
  - [IHasHealth](#ihashealth)
  - [ISelectable](#iselectable)
- [Classes](#classes)
  - [Actor2D](#actor2d)
  - [ActorFollowGrid](#actorfollowgrid)
  - [ActorTarget](#actortarget)
  - [AnimationController](#animationcontroller)
  - [Energy](#energy)
  - [RandomSpeed](#randomspeed)
  - [RenderDepthUpdate](#renderdepthupdate)
  - [Selectable](#selectable)
  - [Sensor2D](#sensor2d)
  - [Sensors](#sensors)
- [Custom Actor Example](#custom-actor-example)
- [Usage Examples](#usage-examples)
  - [Moving an Actor using Keyboard Input](#moving-an-actor-using-keyboard-input)
  - [Making an Actor Jump](#making-an-actor-jump)
  - [Making a Actor Perform a Melee Attack](#making-a-actor-perform-a-melee-attack)
  - [Applying Damage on Contact](#applying-damage-on-contact)
  - [Detecting Another Actor with Sensor2D](#detecting-another-actor-with-sensor2d)

## Overview
The system is built around the concept of actors, which are 2D game objects that can represent characters, enemies, NPCs, or any other entities that require specific behaviors and interactions in a game.

## Interfaces

### IActor

The `IActor` interface defines the basic properties and events of an actor. Any 2D game object that behaves as an actor should implement this interface. The `IActor` interface provides the following properties and events:

- `bool isAlive`: Indicates whether the actor is currently alive.
- `Vector3 position`: The position of the actor in 3D space.
- `event Action<IActor, ActorState> stateChanged`: An event that is raised whenever the state of the actor changes. The `ActorState` enum represents different states an actor can have, such as idle, moving, attacking, etc.

### IActorTimedAction

The `IActorTimedAction` interface allows an actor to perform a timed action, such as waiting or performing an action for a specific duration. This interface can be implemented by actors that require time-based behavior.
It has the following method:

- `void PerformTimedAction(float duration)`: Starts the timed action for the specified duration.

### IAnimationController

The `IAnimationController` interface allows an actor to control its animations. Actors that have animations can implement this interface to manage their animation states.
The interface has the following methdos for animation controls:

- `void PlayAnimation(string animationName)`: Plays the specified animation.
- `void StopAnimation()`: Stops the current animation.

### IHasHealth

The `IHasHealth` interface allows an actor to have a health system. Actors that can take damage and be destroyed should implement this interface to manage their health.
The following properties control the actor's health:

- `float currentHealth`: The current health of the actor.
- `float maxHealth`: The maximum health the actor can have.

### ISelectable

The `ISelectable` interface allows a game object to be selectable. This is useful for implementing interactions with specific objects in the game world.
This system is handled with the methods:

- `void Select()`: Handles the selection of the game object.
- `void Deselect()`: Handles the deselection of the game object.

## Classes

### Actor2D

The `Actor2D` class is a component that represents a 2D actor in the game world. It implements the `IActor` interface and provides functionalities for movement, animations, and health management.

Public Properties

- `health`: Gets the current health of the actor. You can use this property to access the actor's health and perform actions like healing or taking damage.
- `needsHealing`: Gets a boolean value indicating if the actor needs healing. It returns true if the actor is alive and its health is not full.
- `movementSpeed`: Represents the movement speed of the actor.
- `verticalMovementDampening`: Represents the dampening effect applied to the vertical movement of the actor.
- `target`: Represents the current target of the actor. It allows you to set a target for the actor to move towards or perform actions on.
- `lookDirection`: Represents the direction the actor is looking at.
- `actionPivot`: Represents the pivot point for performing actions.

Public Methods

- `MakeInactive()`: Makes the actor inactive by disabling its target and setting its state to Disabled.
- `Disable()`: Disables the actor by making it inactive and optionally destroying its GameObject.
- `DisableAndFadeOut(float time)`: Disables the actor, fades out its animation over the specified time, and optionally destroys its GameObject.
- `Kill()`: Kills the actor by applying damage equal to its current health, effectively reducing its health to zero.
- `Reset()`: Resets the actor to its initial state by resetting its health, disabling its target, and setting its state to Idle.
- `ApplyDamage(float damage)`: Applies damage to the actor, reducing its health by the specified amount.
- `ApplyHealing(float amount)`: Applies healing to the actor, increasing its health by the specified amount.
- `SetTarget(Transform newTarget, float distance = TARGET_DISTANCE, bool determined = false)`: Sets a new target for the actor as a Transform, such as walking to a building or a flag.
- `SetTarget(Vector2 position, float distance = TARGET_DISTANCE, bool determined = false)`: Sets a new target for the actor as a position in 2D space.
- `SetTarget(Actor2D otherActor, bool determined = false)`: Sets another actor as the actor's new target, such as an enemy or friendly unit to be healed.
- `SetMovement(Vector2 moveDirection)`: Sets the actor's movement direction using a 2D Vector.
- `Freeze()`: Makes the actor stand still at the current position and prevents it from being moved around by other actors.
- `UnFreeze()`: Unfreezes the actor, allowing it to move and be affected by physics.
- `StopMovement()`: Stops the actor's movement, disables the target, and sets the state to Idle.
- `TakeAction(Actor2D targetActor)`: Initiates the actor to take an action on a target actor, such as attacking or using a skill.
- `TakeAction(IEnumeratedAction enumeratedAction)`: Initiates the actor to take an action as part of an IEnumeratedAction sequence.


### ActorFollowGrid

The `ActorFollowGrid` class is a component that allows an actor to follow a grid-based path. It is useful for implementing movement along a grid, such as in tile-based games.

### ActorTarget

The `ActorTarget` class is a component that represents a target for an actor. It can be used to indicate a position or game object that the actor should interact with.

Public Properties

- `isReached`: Gets a value indicating whether the Actor has reached its target.
- `hasTarget`: Gets a value indicating whether the Actor has a target assigned.
- `hasActorTarget`: Gets a value indicating whether the Actor's target is another Actor.

Public Methods

- `Update()`: Updates the ActorTarget, making the Actor2D follow the target if applicable.
- `GetFinalTargetPosition()`: Gets the final target position, depending on the type of the target.
- `GetCurrentTargetLocation()`: Gets the current target location, considering any ongoing path following.
- `SetTarget(Vector2 targetPos, float targetDistance, bool newDetermination = false)`: Sets the target as a Vector2 position.
- `SetTarget(Transform targetTransform, float targetDistance, bool newDetermination = false)`: Sets the target as a Transform object.
- `SetTarget(Actor2D otherActor, float targetDistance, bool newDetermination = false)`: Sets the target as another Actor2D object.
- `DisableTarget()`: Disables the current target and clears any target-related data.
- `SetPathField(IPathField field)`: Sets the pathfinding field to be used for calculating the path to the target.

### AnimationController

The `AnimationController` class is a component that controls the animations of an actor. It implements the `IAnimationController` interface.

Public Properties

- `displayObject`: The GameObject that holds the Animator component for the animations.
- `additionalSprites`: Additional SpriteRenderers to change color when fading.
- `excludeRendererFromEffects`: Renderers to exclude from color effects.

Public Methods

- `SetVerticalLookDirection(float direction)`: Sets the vertical look direction of the avatar animation.
- `SetMaterialColor(Color color)`: Sets the color of all sprite children.
- `FadeIn(float time = 1f)`: Initiates a fade-in effect for the avatar animation.
- `FadeOut(float time = 1f)`: Initiates a fade-out effect for the avatar animation.
- `FadeOutAfterDeath()`: Initiates a fade-out effect after the avatar's death.
- `Reset()`: Resets the animation controller to the default state.
- `SetAnimatorController(RuntimeAnimatorController controller)`: Sets the animator controller for the avatar animation.
- `SetSpeed(float speed)`: Sets the playback speed of the avatar animation.
- `ResetSpeed()`: Resets the playback speed of the avatar animation to normal (1).

### Energy

The `Energy` class is a component that represents an energy system for an actor. It can be used to manage an actor's energy level and perform energy-based actions.

Public Properties

- `current`: Gets the current energy value, clamped between 0 and the maximum value.
- `max`: Gets the maximum energy value.
- `missingAmount`: Gets the amount of energy missing to reach the maximum value.
- `proportion`: Gets the proportion of current energy to the maximum value (0 to 1).
- `isFull`: Gets a value indicating whether the energy is at its maximum capacity.
- `isEmpty`: Gets a value indicating whether the energy is empty (reached the minimum value).
- `range`: Gets the range of energy values (difference between the maximum and minimum).

Public Methods:

- `AddPortion(float portion)`: Adds a portion of the maximum possible energy to the current energy value.
- `AddFull()`: Adds the maximum possible energy to the current energy value.
- `Add(float amount)`: Adds a specified amount to the current energy value.
- `LosePortion(float portion)`: Loses a portion of the maximum possible energy from the current energy value.
- `LoseAll()`: Loses all the current energy.
- `Lose(float amount)`: Loses a specified amount from the current energy value.
- `Reset()`: Resets the current energy value to its starting value.
- `Scale(float factor)`: Scales the maximum and current energy values by a given factor for balancing.
- `GetAmountThatWillBeAdded(float maxPossibleAmount)`: Gets the maximum possible amount that can be added to the current energy value.
- `ToString()`: Returns the current energy value and the maximum energy value as a formatted string.

### RandomSpeed

The `RandomSpeed` class is a component that adds a random speed boost to an `Actor2D`'s movement speed upon initialization.

Public Properties

- `multiplier`: The multiplier for the random speed boost. The random value will be in the range of (1 * multiplier) to (10 * multiplier).

### RenderDepthUpdate

The `RenderDepthUpdate` class is a component that updates the rendering depth of a game object based on its position in the scene.

Public Properties

- `bool toBackgroundWhenDead`: If true, move the game object to the background when it becomes dead (inactive).
- `float offset`: The offset value to apply to the rendering depth.

Public Methods

- `UpdatePosition`: Updates the position of the GameObject with the given current position.

### Selectable

The `Selectable` class is a component that allows game objects to be selectable and provides settings for searching for the selectable interface in the current object or its parent/children.

Public Properties

- `ISelectable selectable`: Reference to the selectable interface of the game object.
- `SearchForSelectable searchType`: The search option for finding the selectable interface.

### Sensor2D

The `Sensor2D` class is a component that detects and tracks other actors within its 2D trigger collider and raises events for detected and lost actors.

Public Properties

- `string searchTag`: The tag to filter detected actors. If empty, all actors are detected.
- `List<IActor> actors`: List of actors detected by the sensor.
- `bool ActorsDetected`: Flag to indicate whether the sensor has detected any actors.

### Sensors

The `Sensors` class is a component that detects and tracks other actors within its 2D trigger collider and raises events for detected and lost actors.
The detected actors are filtered based on their tags (only actors with a specified tag are detected).
This class represents actors using the "Actor2D" class and provides public methods to get the most wounded and nearest actors from the detected list.

Public Properties

- `string Tag`: The tag to filter detected actors. Only actors with this tag will be detected.
- `List<Actor2D> actors`: List of actors detected by the sensor.
- `bool ActorsDetected`: Flag to indicate whether the sensor has detected any actors.

Public Methods

- `Actor2D GetMostWoundedActor()`: Gets the most wounded actor from the detected actors list.
- `Actor2D GetNearestActor()`: Gets the nearest actor from the detected actors list.

## Custom Actor Example

To create a custom actor in your game, you can follow these steps:

1. Create a new 2D game object in the Unity editor.
2. Attach a `Rigidbody2D` component to the gameobject as `Actor2D` uses it
3. Attach a `Collider2D` component to the gameobject as `Actor2D uses it
4. Attach the `Actor2D` component to the game object to make it an actor.
5. Optionally, attach other components such as `AnimationController`, `Energy`, or custom action scripts to provide additional functionalities to the actor.

## Usage Examples

### Moving an Actor using Keyboard Input
You can enable keyboard input to control the movement of an Actor2D using the Move method. Here's an example of how to move an actor using arrow keys:

```csharp
using UnityEngine;
using egads.system.actors;

public class ActorMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Actor2D actor;

    private void Start()
    {
        actor = GetComponent<Actor2D>();
    }

    private void Update()
    {
        // Get horizontal and vertical input values
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate movement direction based on input
        Vector2 moveDirection = new Vector2(horizontalInput, verticalInput).normalized;

        // Set the actor's movement direction using a 2D Vector
        actor.SetMovement(moveDirection * moveSpeed);
    }
}
```
In this example, the `ActorMovement` script is attached to an `Actor2D` game object. The `Update` method retrieves horizontal and vertical input values using `Input.GetAxis` to detect arrow key inputs. The calculated movement direction is then normalized and passed to the `SetMovement` method of the `Actor2D` component, allowing the actor to move in the specified direction at a constant speed, based on the input provided.

### Making an Actor Jump
The provided code demonstrates how to make an actor perform a jump in a 2D Unity game using a custom timed action.

```csharp
using UnityEngine;
using egads.system.actors;

// Custom actor action for player
public class JumpAction : IActorTimedAction
{
    // Implementations of IActorTimedAction
    public float range => jumpRange;
    public float cooldown => jumpCooldown;

    // Jump range represents the distance the actor can jump.
    private float jumpRange;

    // Jump cooldown is the time period between consecutive jumps.
    private float jumpCooldown;

    // The force to apply when performing the jump.
    private Vector2 jumpForce;

    // Reference to the Rigidbody2D component of the actor.
    private Rigidbody2D rigidbody;

    public JumpAction(float jumpRange, float jumpCooldown, Vector2 jumpForce, Rigidbody2D rigidbody)
    {
        this.jumpRange = jumpRange;
        this.jumpCooldown = jumpCooldown;
        this.jumpForce = jumpForce;
        this.rigidbody = rigidbody;
    }

    // Executes the jump action by applying the jump force to the actor's Rigidbody2D.
    public void Execute()
    {
        rigidbody.AddForce(jumpForce, ForceMode2D.Impulse);
    }
}
```
The `JumpAction` class is a custom implementation of the `IActorTimedAction` interface. It defines the jump action for the actor, including the jump range, jump cooldown, jump force, and the actor's `Rigidbody2D`. The range property returns the jump range, representing the maximum distance the actor can jump. The cooldown property returns the jump cooldown, which is the time period between consecutive jumps. The `Execute()` method is responsible for executing the jump action. It applies the jump force to the actor's `Rigidbody2D`, causing it to jump with an impulse force.
```csharp
using UnityEngine;
using egads.system.actors;

public class JumpExample : MonoBehaviour
{
    // The force applied to the actor when jumping.
    public float jumpForce = 5f;

    // The distance the actor can jump.
    public float jumpRange = 1f;

    // The cooldown time between consecutive jumps.
    public float jumpCooldown = 1f;

    private Actor2D actor;
    private Rigidbody2D rb;

    private void Start()
    {
        actor = GetComponent<Actor2D>();
        rb = GetComponent<Rigidbody2D>();
        JumpAction jumpAction = new JumpAction(jumpRange, jumpCooldown, new Vector2(0, jumpForce), rb);
        actor.action = jumpAction;
    }

    private void Update()
    {
        // Check for jump input (e.g., space key) and if the actor is grounded.
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            // Tell the actor to take the assigned action, which in this case, is the jump action.
            actor.TakeAction(actor);
        }
    }

    // Check if the actor is grounded.
    private bool IsGrounded()
    {
        float distanceToGround = 0.1f; // Adjust this based on your actor's size and collider.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, distanceToGround, LayerMask.GetMask("Ground"));
        return hit.collider != null;
    }
}
```
The JumpExample class is attached to the player `GameObject` in the scene. It exposes public variables to set the jump force, jump range, and jump cooldown in the Unity Inspector. In the `Start()` method, the JumpAction is created with the specified jump range, jump cooldown, jump force, and the player's `Rigidbody2D`. It then assigns this action to the `actor.action`, making the actor aware of the jump action. In the `Update()` method, the script checks for jump input (e.g., space key) and whether the actor is grounded using the `IsGrounded()` method. If both conditions are met, the `actor.TakeAction(actor)` method is called, which triggers the actor to execute the assigned jump action.

### Making a Actor Perform a Melee Attack
You can create a melee attack behavior for an `Actor2D` by detecting nearby actors within a certain range and applying damage to them. Here's an example script for a melee attack:

```csharp
using UnityEngine;
using egads.system.actors;

public class MeleeAttack : MonoBehaviour
{
    public float attackRange = 1.5f;
    public float attackDamage = 15f;

    private Actor2D actor;

    private void Start()
    {
        actor = GetComponent<Actor2D>();
    }

    private void Update()
    {
        // Check for attack input (e.g., left mouse button)
        if (Input.GetButtonDown("Fire1"))
        {
            PerformMeleeAttack();
        }
    }

    private void PerformMeleeAttack()
    {
        // Detect nearby actors within the attack range
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRange);

        // Apply damage to detected actors
        foreach (Collider2D collider in colliders)
        {
            Actor2D otherActor = collider.GetComponent<Actor2D>();
            if (otherActor != null && otherActor != actor)
            {
                otherActor.ApplyDamage(attackDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a visual representation of the attack range in the Unity editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
```
### Applying Damage on Contact
You can create an `Actor2D` that inflicts damage to other actors when they collide. To achieve this, you can use Unity's `OnCollisionEnter2D` method to detect collisions and apply damage:

```csharp
using UnityEngine;
using egads.system.actors;

public class DamageOnContact : MonoBehaviour
{
    public float damageAmount = 10f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision is with an actor
        Actor2D otherActor = collision.collider.GetComponent<Actor2D>();
        if (otherActor != null)
        {
            // Apply damage to the other actor
            otherActor.ApplyDamage(damageAmount);
        }
    }
}
```

### Detecting Another Actor with Sensor2D
The `Sensor2D` component allows an `Actor2D` to detect and track other actors within its 2D trigger collider. Here's an example of how to use Sensor2D to detect nearby actors:

```csharp
using UnityEngine;
using egads.system.actors;

public class ActorDetection : MonoBehaviour
{
    private Actor2D actor;
    private Sensor2D sensor;

    private void Start()
    {
        actor = GetComponent<Actor2D>();
        sensor = GetComponent<Sensor2D>();

        // Subscribe to the sensor event to handle detected and lost actors
        sensor.sensorEvent += OnSensorEvent;
    }

    private void OnSensorEvent(SensorEvent eventType, IActor otherActor)
    {
        // Check if the detected actor is of type MonoBehaviour
        MonoBehaviour monoBehaviour = otherActor as MonoBehaviour;
        if (monoBehaviour != null)
        {
            // Successfully cast to MonoBehaviour, now you can access the gameObject property
            if (eventType == SensorEvent.ActorDetected)
            {
                Debug.Log("Detected actor: " + monoBehaviour.gameObject.name);
            }
            else if (eventType == SensorEvent.ActorLeft)
            {
                Debug.Log("Lost actor: " + monoBehaviour.gameObject.name);
            }
        }
        else
        {
            // The detected actor is not of type MonoBehaviour, so you cannot access the gameObject property.
            Debug.Log("Detected actor is not a MonoBehaviour.");
        }
    }
}
```

With the Actors System, you have a powerful set of tools to manage and control actors in your Unity game. Experiment with the different interfaces and classes to create engaging and interactive characters and entities.
