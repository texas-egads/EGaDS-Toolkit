# Camera System User Guide

This camera system provided in this codebase offers a comprehensive set of features to manage camera behavior in Unity projects. This system includes two main components: `CameraShake` and `CameraSmoothFollow2D`.

- [Classes](#classes)
  - [CameraShake](#camerashake)
  - [CameraSmoothFollow2D](#camerasmoothfollow2d)
  - [CameraTargetThis](#cameratargetthis)
- [Usage Examples](#usage-examples)
  - [CameraShake Implementation](#camerashake-implementation)
  - [CameraSmoothFollow2D Implementation](#camerasmoothfollow2d-implementation)
  - [CameraTarget Implementation](#cameratarget-implementation)

## Classes

### CameraShake

The `CameraShake` component handles camera shaking functionality, allowing you to simulate visual effects such as earthquakes, impacts, or explosions. It smoothly applies a random positional offset to the camera, creating a shaking effect. The shaking intensity and rate of decrease can be adjusted.

Public Properties

- `float shakeAmount`: The intensity of the camera shake.
- `float decreaseFactor`: The rate at which the shake decreases over time.

Public Methods

- `void Shake(float amount)`: Initiates camera shaking with a given intensity.

### CameraSmoothFollow2D

The `CameraSmoothFollow2D` component provides smooth 2D camera following behavior for single and multiple targets. It allows the camera to track moving objects while maintaining a customizable buffer zone and ensuring that the camera stays within specified level boundaries.

Public Properties

- `Transform target`: Sets a single target for the camera to follow.
- `List<Transform> targets`: Sets a list of targets for the camera to follow.
- `MoveDirection direction`: Specifies the camera's movement direction (horizontal, vertical, or both).
- `float horizontalFollowSpeed`: The horizontal speed at which the camera follows the target(s).
- `float verticalFollowSpeed`: The vertical speed at which the camera follows the target(s).
- `float smoothEdge`: The distance from the screen edges where camera movement becomes smooth.
- `float minimumOrthographicSize`: The minimum size of the camera's orthographic view.
- `float zoomSpeed`: The speed at which the camera zooms in or out.
- `bool canMoveUp`, `canMoveDown`, `canMoveLeft`, `canMoveRight`: Indicate whether the camera can move in different directions.

Public Methods

- `void Zoom(float delta)`: Zooms the camera by a specified amount.
- `void SetOrthographicSizeAtPivot(float size, Vector2 pivot, Vector2 directionToCamera, float oldSize)`: Sets the orthographic size while keeping a pivot point at a certain position.
- `void Translate(Vector2 delta)`: Translates the camera by a specified delta in screen space.
- `void SetPosition(Vector2 pos)`: Sets the position of the camera.
- `void SetBoundaries(LevelBoundaries levelBoundaries)`: Sets the level boundaries for the camera movement.
- `void SetOrthoSize(float size)`: Sets the orthographic size of the camera and applies level boundaries.
- `void SetTargetOrthoSize(float size)`: Sets the target orthographic size without applying level boundaries.
- `void ScaleFollowSpeed(float scale)`: Scales the horizontal and vertical follow speeds by a specified factor.
- `void ResetFollowSpeed()`: Resets the horizontal and vertical follow speeds to their original values.

## Usage Examples

### CameraShake Implementation
Attach the `CameraShake` component to your camera object to enable camera shaking. You can then trigger camera shaking using the `Shake(float amount)` method:

```csharp
// Shake the camera with an intensity of 0.5
CameraShake cameraShake = GetComponent<CameraShake>();
cameraShake.Shake(0.5f);
```
   
### CameraSmoothFollow2D Implementation
Attach the `CameraSmoothFollow2D` component to your camera object to enable smooth 2D camera following. Set the target(s) for the camera using the `target` or `targets` properties:

```charp
// Set a single target for the camera to follow
CameraSmoothFollow2D cameraFollow = GetComponent<CameraSmoothFollow2D>();
cameraFollow.target = playerTransform;

// Set a list of targets for the camera to follow
cameraFollow.targets = new List<Transform> { player1Transform, player2Transform };
```

Utilize the various methods provided by the `CameraSmoothFollow2D` component to customize camera behavior:

```charp
// Zoom in the camera
cameraFollow.Zoom(-0.5f);

// Translate the camera
cameraFollow.Translate(new Vector2(10f, 0f));

// Set camera position
cameraFollow.SetPosition(new Vector2(0f, 5f));

// Set level boundaries for camera movement
cameraFollow.SetBoundaries(levelBoundaries);

// Set orthographic size and apply bounds
cameraFollow.SetOrthoSize(5f);

// Scale follow speeds
cameraFollow.ScaleFollowSpeed(1.5f);

// Reset follow speeds to defaults
cameraFollow.ResetFollowSpeed();
```
### CameraTarget Implementation
Attach the `CameraTargetThis` script to a `GameObject` to make it the target of a `CameraSmoothFollow2D` camera. Remember to adjust the public properties and methods as needed to fit your specific game's camera behavior requirements.

This camera system offers a flexible and versatile solution for managing camera behavior in Unity projects. By leveraging the `CameraShake` and `CameraSmoothFollow2D` components, you can easily implement dynamic camera effects and smooth object tracking within your games.
