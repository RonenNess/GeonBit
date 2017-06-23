# Glossary

Before diving deeper into *GeonBit*, lets define some basic concepts (these are relevant to most ECS engines out there):

## GameObject

Game Objects are our main entities in the Engine. They are the ```Entities``` of the Entity-Component-System model. 
Every element of the game is implemented by a Game Object with components attached to it, from monsters to scripting and sound effects.

There is only one type of a *GameObject*, the difference between objects reside in the component types attached to them.
For example, if you want to place trees in your game, you'll probably create a *GameObject* with a 3D model of a tree + physical body attached to it.

Every *GameObject* have a list of child *GameObject*s, and a 3D scene node that represent its transformations in 3d space (eg position, rotation, scale etc).


## Components

*Components* are the logic pieces you attach to *GameObjects*, like a 3d model to render, a physicla body, sound effect to play, etc.

A *GameObject* without any components is just a node in the Scene, it has transformations and a 3D position and can hold children, but it doesn't do much without any *Components*.  To make a *GameObject* a meaningful part of your game, you attach *Components* to it.

Most components are independent and do not communicate with each other, but there are some exceptions that depeand on each other or even affect the transformations of the *GameObject* itself (for example, physics-related components often change the position & rotation of the *GameObject* containing them).

Note that *Components* are also the objects you will implement the most. For example, to create an AI to control your game monsters, you'll probably write some sort of an ```NPCs-Controller``` component and attach it to your monsters ```Game Objects```.


## SceneNode

Every *GameObject* has a scene node that represent its 3d transformations, eg position, scale, rotation, etc.
Unlike the *GameObject* that only has one type, there are many types of ```SceneNode```s that behave differently and are optimized for different purposes. We will cover those later.


## Prototypes

Since different objects in the game are made of *GameObject* with components attached to them, you probably want a way to define a game-specific type and create instances of it with all the components it needs. To do so, we use ```Prototypes```.

A ```Prototype``` is an instance of a *GameObject* you register to the ```Prototypes Manager``` and later you can create clones of it, or 'instanciate' it if you will.


## Scene

A ```Scene``` is a tree of *GameObjects* + some global settings, that represent a level or a 'screen' in your game. 

```Scenes``` can be easily loaded / unload to switch between levels and scenery.

