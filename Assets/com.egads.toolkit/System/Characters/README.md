# Characters System User Guide
The Characters System is a collection of interfaces and classes designed to facilitate the management of 2D characters (game objects) in Unity. It provides a set of components and functionalities that can be used to create and control characters with various behaviors such as movement, health, rendering, and interaction.

## Table of Contents
- [Overview](#overview)
- [Interfaces](#interfaces)
  - [ICharacter](#icharacter)
  - [ICharacterTimedAction](#icharactertimedaction)
  - [IAnimationController](#ianimationcontroller)
  - [IHasHealth](#ihashealth)
  - [ISelectable](#iselectable)
- [Classes](#classes)
  - [Character2D](#character2d)
  - [CharacterFollowGrid](#characterfollowgrid)
  - [CharacterTarget](#charactertarget)
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
The system is built around the concept of characters, which are 2D game objects that can represent characters, enemies, NPCs, or any other entities that require specific behaviors and interactions in a game.

## Interfaces

### ICharacter

The `ICharacter` interface defines the basic properties and events of a character. Any 2D game object that behaves as a character should implement this interface. The `ICharacter` interface provides the following properties and events:

- `bool isAlive`: Indicates whether the character is currently alive.
- `Vector3 position`: The position of the character in 3D space.
- `event Action<ICharacter, CharacterState> stateChanged`: An event that is raised whenever the state of the character changes. The `CharacterState` enum represents different states an character can have, such as idle, moving, attacking, etc.

### ICharacterTimedAction

The `ICharacterTimedAction` interface allows a character to perform a timed action, such as waiting or performing an action for a specific duration. This interface can be implemented by characters that require time-based behavior.
It has the following method:

- `void PerformTimedAction(float duration)`: Starts the timed action for the specified duration.

### IAnimationController

The `IAnimationController` interface allows a character to control its animations. Characters that have animations can implement this interface to manage their animation states.
The interface has the following methods for animation controls:

- `void PlayAnimation(string animationName)`: Plays the specified animation.
- `void StopAnimation()`: Stops the current animation.

### IHasHealth

The `IHasHealth` interface allows a character to have a health system. Characters that can take damage and be destroyed should implement this interface to manage their health.
The following properties control the character's health:

- `float currentHealth`: The current health of the actor.
- `float maxHealth`: The maximum health the actor can have.

### ISelectable

The `ISelectable` interface allows a game object to be selectable. This is useful for implementing interactions with specific objects in the game world.
This system is handled with the methods:

- `void Select()`: Handles the selection of the game object.
- `void Deselect()`: Handles the deselection of the game object.

## Classes

### Character2D

The `Character2D` class is a component that represents a 2D character in the game world. It implements the `ICharacter` interface and provides functionalities for movement, animations, and health management.

Public Properties

- `health`: Gets the current health of the character. You can use this property to access the character's health and perform actions like healing or taking damage.
- `needsHealing`: Gets a boolean value indicating if the character needs healing. It returns true if the character is alive and its health is not full.
- `movementSpeed`: Represents the movement speed of the character.
- `verticalMovementDampening`: Represents the dampening effect applied to the vertical movement of the character.
- `target`: Represents the current target of the character. It allows you to set a target for the character to move towards or perform actions on.
- `lookDirection`: Represents the direction the character is looking at.
- `actionPivot`: Represents the pivot point for performing actions.

Public Methods

- `MakeInactive()`: Makes the character inactive by disabling its target and setting its state to Disabled.
- `Disable()`: Disables the character by making it inactive and optionally destroying its GameObject.
- `DisableAndFadeOut(float time)`: Disables the character, fades out its animation over the specified time, and optionally destroys its GameObject.
- `Kill()`: Kills the character by applying damage equal to its current health, effectively reducing its health to zero.
- `Reset()`: Resets the character to its initial state by resetting its health, disabling its target, and setting its state to Idle.
- `ApplyDamage(float damage)`: Applies damage to the character, reducing its health by the specified amount.
- `ApplyHealing(float amount)`: Applies healing to the character, increasing its health by the specified amount.
- `SetTarget(Transform newTarget, float distance = TARGET_DISTANCE, bool determined = false)`: Sets a new target for the character as a Transform, such as walking to a building or a flag.
- `SetTarget(Vector2 position, float distance = TARGET_DISTANCE, bool determined = false)`: Sets a new target for the character as a position in 2D space.
- `SetTarget(Character2D otherCharacter, bool determined = false)`: Sets another character as the character's new target, such as an enemy or friendly unit to be healed.
- `SetMovement(Vector2 moveDirection)`: Sets the character's movement direction using a 2D Vector.
- `Freeze()`: Makes the character stand still at the current position and prevents it from being moved around by other characters.
- `UnFreeze()`: Unfreezes the character, allowing it to move and be affected by physics.
- `StopMovement()`: Stops the character's movement, disables the target, and sets the state to Idle.
- `TakeAction(Character2D targetCharacter)`: Initiates the character to take an action on a target character, such as attacking or using a skill.
- `TakeAction(IEnumeratedAction enumeratedAction)`: Initiates the character to take an action as part of an IEnumeratedAction sequence.


### CharacterFollowGrid

The `CharacterFollowGrid` class is a component that allows an character to follow a grid-based path. It is useful for implementing movement along a grid, such as in tile-based games.

### CharacterTarget

The `CharacterTarget` class is a component that represents a target for an character. It can be used to indicate a position or game object that the character should interact with.

Public Properties

- `isReached`: Gets a value indicating whether the character has reached its target.
- `hasTarget`: Gets a value indicating whether the character has a target assigned.
- `hasCharacterTarget`: Gets a value indicating whether the character's target is another character.

Public Methods

- `Update()`: Updates the CharacterTarget, making the Character2D follow the target if applicable.
- `GetFinalTargetPosition()`: Gets the final target position, depending on the type of the target.
- `GetCurrentTargetLocation()`: Gets the current target location, considering any ongoing path following.
- `SetTarget(Vector2 targetPos, float targetDistance, bool newDetermination = false)`: Sets the target as a Vector2 position.
- `SetTarget(Transform targetTransform, float targetDistance, bool newDetermination = false)`: Sets the target as a Transform object.
- `SetTarget(Character2D otherCharacter, float targetDistance, bool newDetermination = false)`: Sets the target as another Character2D object.
- `DisableTarget()`: Disables the current target and clears any target-related data.
- `SetPathField(IPathField field)`: Sets the pathfinding field to be used for calculating the path to the target.

### AnimationController

The `AnimationController` class is a component that controls the animations of an character. It implements the `IAnimationController` interface.

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

The `Energy` class is a component that represents an energy system for an character. It can be used to manage an character's energy level and perform energy-based actions.

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

The `RandomSpeed` class is a component that adds a random speed boost to an `Character2D`'s movement speed upon initialization.

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

The `Sensor2D` class is a component that detects and tracks other characters within its 2D trigger collider and raises events for detected and lost characters.

Public Properties

- `string searchTag`: The tag to filter detected characters. If empty, all characters are detected.
- `List<ICharacter> characters`: List of characters detected by the sensor.
- `bool hasCharactersDetected`: Flag to indicate whether the sensor has detected any characters.

### Sensors

The `Sensors` class is a component that detects and tracks other characters within its 2D trigger collider and raises events for detected and lost characters.
The detected charactersare filtered based on their tags (only characterswith a specified tag are detected).
This class represents characters using the `Character2D` class and provides public methods to get the most wounded and nearest characters from the detected list.

Public Properties

- `string Tag`: The tag to filter detected characters. Only characters with this tag will be detected.
- `List<Character2D> characters`: List of characters detected by the sensor.
- `bool hasCharactersDetected`: Flag to indicate whether the sensor has detected any characters.

Public Methods

- `Character2D GetMostWoundedCharacter()`: Gets the most wounded character from the detected characters list.
- `Character2D GetNearestCharacter()`: Gets the nearest character from the detected characters list.

## Custom Character Example

To create a custom character in your game, you can follow these steps:

1. Create a new 2D game object in the Unity editor.
2. Attach a `Rigidbody2D` component to the gameobject as `Character2D` uses it
3. Attach a `Collider2D` component to the gameobject as `Character2D` uses it
4. Attach the `Character2D` component to the game object to make it an actor.
5. Optionally, attach other components such as `AnimationController`, `Energy`, or custom action scripts to provide additional functionalities to the character.

## Usage Examples

### Moving an Character using Keyboard Input
You can enable keyboard input to control the movement of an Character2D using the Move method. Here's an example of how to move a character using arrow keys:

```csharp
using UnityEngine;
using egads.system.characters;

public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Character2D character;

    private void Start()
    {
        character = GetComponent<Character2D>();
    }

    private void Update()
    {
        // Get horizontal and vertical input values
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate movement direction based on input
        Vector2 moveDirection = new Vector2(horizontalInput, verticalInput).normalized;

        // Set the character's movement direction using a 2D Vector
        character.SetMovement(moveDirection * moveSpeed);
    }
}
```
In this example, the `CharacterMovement` script is attached to an `Character2D` game object. The `Update` method retrieves horizontal and vertical input values using `Input.GetAxis` to detect arrow key inputs. The calculated movement direction is then normalized and passed to the `SetMovement` method of the `Character2D` component, allowing the character to move in the specified direction at a constant speed, based on the input provided.

### Making an Character Jump
The provided code demonstrates how to make an character perform a jump in a 2D Unity game using a custom timed action.

```csharp
using UnityEngine;
using egads.system.characters;

// Custom character action for player
public class JumpAction : ICharacterTimedAction
{
    // Implementations of ICharacterTimedAction
    public float range => jumpRange;
    public float cooldown => jumpCooldown;

    // Jump range represents the distance the character can jump.
    private float jumpRange;

    // Jump cooldown is the time period between consecutive jumps.
    private float jumpCooldown;

    // The force to apply when performing the jump.
    private Vector2 jumpForce;

    // Reference to the Rigidbody2D component of the character.
    private Rigidbody2D rigidbody;

    public JumpAction(float jumpRange, float jumpCooldown, Vector2 jumpForce, Rigidbody2D rigidbody)
    {
        this.jumpRange = jumpRange;
        this.jumpCooldown = jumpCooldown;
        this.jumpForce = jumpForce;
        this.rigidbody = rigidbody;
    }

    // Executes the jump action by applying the jump force to the character's Rigidbody2D.
    public void Execute()
    {
        rigidbody.AddForce(jumpForce, ForceMode2D.Impulse);
    }
}
```
The `JumpAction` class is a custom implementation of the `ICharacterTimedAction` interface. It defines the jump action for the character, including the jump range, jump cooldown, jump force, and the character's `Rigidbody2D`. The range property returns the jump range, representing the maximum distance the character can jump. The cooldown property returns the jump cooldown, which is the time period between consecutive jumps. The `Execute()` method is responsible for executing the jump action. It applies the jump force to the character's `Rigidbody2D`, causing it to jump with an impulse force.
```csharp
using UnityEngine;
using egads.system.characters;

public class JumpExample : MonoBehaviour
{
    // The force applied to the character when jumping.
    public float jumpForce = 5f;

    // The distance the character can jump.
    public float jumpRange = 1f;

    // The cooldown time between consecutive jumps.
    public float jumpCooldown = 1f;

    private Character2D character;
    private Rigidbody2D rb;

    private void Start()
    {
        character = GetComponent<Character2D>();
        rb = GetComponent<Rigidbody2D>();
        JumpAction jumpAction = new JumpAction(jumpRange, jumpCooldown, new Vector2(0, jumpForce), rb);
        character.action = jumpAction;
    }

    private void Update()
    {
        // Check for jump input (e.g., space key) and if the character is grounded.
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            // Tell the character to take the assigned action, which in this case, is the jump action.
            character.TakeAction(character);
        }
    }

    // Check if the character is grounded.
    private bool IsGrounded()
    {
        float distanceToGround = 0.1f; // Adjust this based on your character's size and collider.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, distanceToGround, LayerMask.GetMask("Ground"));
        return hit.collider != null;
    }
}
```
The JumpExample class is attached to the player `GameObject` in the scene. It exposes public variables to set the jump force, jump range, and jump cooldown in the Unity Inspector. In the `Start()` method, the JumpAction is created with the specified jump range, jump cooldown, jump force, and the player's `Rigidbody2D`. It then assigns this action to the `character.action`, making the character aware of the jump action. In the `Update()` method, the script checks for jump input (e.g., space key) and whether the character is grounded using the `IsGrounded()` method. If both conditions are met, the `character.TakeAction(character)` method is called, which triggers the character to execute the assigned jump action.

### Making a Character Perform a Melee Attack
You can create a melee attack behavior for an `Character2D` by detecting nearby characters within a certain range and applying damage to them. Here's an example script for a melee attack:

```csharp
using UnityEngine;
using egads.system.characters;

public class MeleeAttack : MonoBehaviour
{
    public float attackRange = 1.5f;
    public float attackDamage = 15f;

    private Character2D character;

    private void Start()
    {
        character = GetComponent<Character2D>();
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
        // Detect nearby characters within the attack range
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRange);

        // Apply damage to detected characters
        foreach (Collider2D collider in colliders)
        {
            Character2D otherCharacter = collider.GetComponent<Character2D>();
            if (otherCharacter != null && otherCharacter != character)
            {
                otherCharacter.ApplyDamage(attackDamage);
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
You can create an `Character2D` that inflicts damage to other characters when they collide. To achieve this, you can use Unity's `OnCollisionEnter2D` method to detect collisions and apply damage:

```csharp
using UnityEngine;
using egads.system.characters;

public class DamageOnContact : MonoBehaviour
{
    public float damageAmount = 10f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision is with an character
        Character2D otherCharacter = collision.collider.GetComponent<Character2D>();
        if (otherCharacter != null)
        {
            // Apply damage to the other character
            otherCharacter.ApplyDamage(damageAmount);
        }
    }
}
```

### Detecting Another Character with Sensor2D
The `Sensor2D` component allows an `Character2D` to detect and track other characters within its 2D trigger collider. Here's an example of how to use Sensor2D to detect nearby characters:

```csharp
using UnityEngine;
using egads.system.characters;

public class CharacterDetection : MonoBehaviour
{
    private Character2D character;
    private Sensor2D sensor;

    private void Start()
    {
        character = GetComponent<Character2D>();
        sensor = GetComponent<Sensor2D>();

        // Subscribe to the sensor event to handle detected and lost characters
        sensor.sensorEvent += OnSensorEvent;
    }

    private void OnSensorEvent(SensorEvent eventType, ICharacter otherCharacter)
    {
        // Check if the detected character is of type MonoBehaviour
        MonoBehaviour monoBehaviour = otherCharacter as MonoBehaviour;
        if (monoBehaviour != null)
        {
            // Successfully cast to MonoBehaviour, now you can access the gameObject property
            if (eventType == SensorEvent.CharacterDetected)
            {
                Debug.Log("Detected character: " + monoBehaviour.gameObject.name);
            }
            else if (eventType == SensorEvent.CharacterLeft)
            {
                Debug.Log("Lost character: " + monoBehaviour.gameObject.name);
            }
        }
        else
        {
            // The detected character is not of type MonoBehaviour, so you cannot access the gameObject property.
            Debug.Log("Detected character is not a MonoBehaviour.");
        }
    }
}
```

With the Characters System, you have a powerful set of tools to manage and control characters in your Unity game. Experiment with the different interfaces and classes to create engaging and interactive characters and entities.
