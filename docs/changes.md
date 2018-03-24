# ChangeLog

## 0.1.0.8

- Fixed bug in getting GameObject's physical body via the `PhysicalBody` alias.
- Fixed ray casting for physical bodies that are not rigid, eg static and kinematic bodies.
- Added 'FrameId' to time manager.
- Added 'SyncUpdateWith' to RigidBody to fix rigid body updating + camera jerking artifact.
- Added 'AutoUpdate' property to camera, for better control over when it updates.
- Added basic renderer with material override, so that animators could be used on sprites as well.
- Added emissive color to override material.
- Fixed sprite sheet default constructor.
- Added camera component FieldOfView.

## 0.1.0.7

- Updated GeonBit.UI to version 3.1.0.1.

## 0.1.0.6

- Added normal mapped material.
- Added support in multiple vertex types for combined mesh.
- Removed usage of deprecated Matrix.Rotation and Matrix.Scale getters.

Note: closing version before big dev iteration.

## 0.1.0.5

- Improvements to lights manager and basic lit effect.


## 0.1.0.4

- Fixed bug with added-to-scene event.
- Increased combined mesh max capacity + added assert if exceed (instead of just not drawing parts).
- Fixed bug with changing position of static physical objects. 


## 0.1.0.3

- Added lit material and lights manager.
- Better normal transformations in combined meshes.
- Fixed cloning of physical objects and collision shapes.
- Added 'EnableSimulation' boolean to physical bodies.
- Added static physical body (so now we have Rigid Body, Kinematic Body, and Static body).
- Improved static and kinematic body performance, and physics in general.
- Added compound physical collision shape.
- Moved default physics collision group / masks to Core, and added default group and masks to different physical body types.
- Added dirty flags to material properties.
- Removed LightingEnable property from materials (material should either be lit or unlit, it shouldn't be configurable - design choice).
- Fixed combined meshes bounding box / sphere bugs.
- Removed duplicated AddToScene event.
- Fixed bug in cloning model renderer materials.
- Optimized override materials memory consumption.


## 0.1.0.2

- Fixed physics debug renderings.
- Renamed physical body to Rigid body.
- Added kinematic body component.
- Added more options to physics world simulation.
- Added physics height map shape.
- Updated to Bullet 0.10.0 version.
- Refactored some node-related functions and API.
- Added OnTransformationUpdate event.
- Disable physics rendering by default even in debug mode.
- Improved Combined Meshes performance by using vertex / index buffers.
- Optimized some lists-to-array conversion by using custom list that don't copy memory.


## 0.0.0.4

- Minor fixes in ```TileMap``` component.
- Added options to iterate tiles and batches in ```TileMap```.
- Fixed octree cloning.
- Optimized game object cloning.
- Added option to disable update events for GameObjects that don't need it.
- Added ```SamplerStates``` getter.
- Added Combined Meshes Renderer.
- Added ```Transform``` to scene node to process Matrices.
- Added option to force node to update its transformations.
- Optimized octree to not add empty nodes to octree.
- Optimized bounding box / sphere culling nodes to cull when empty.
- Added lots of useful functions in ```Game``` to set resolution and fullscreen.
- Added option to set relative step in sprite animation.
- Added more getters to resource manager.


## 0.0.0.3

First stable version release, which includes the following features / classes:

- UI
- - GeonBit.UI v3.0.0.1
- GeonBitGame
- Managers
- - Application
- - ConfigStorage
- - Diagnostics
- - GameFiles
- - GameInput
- - Graphics
- - Plugins
- - Prototypes
- - Sounds
- - GameTime
- Resources Manager
- Graphics
- - Background
- - Skybox
- - Model
- - Skinned Model
- - Composite Model
- - Billboard
- - Sprite
- - Shapes
- Misc
- - TileMap
- - Editor Controller
- - Time To Live
- Particles System
- - CPU Particles
- - Animators
- Physics
- - Physical body with all basic shapes
- - Collision groups and filters
- - Ethereal objects
- - Static objects
- - Raytesting
- Sound
- - Sound effects
- - 3D sounds
- - Background music