# ECS

ECS stands for "Entity–component–system" (https://en.wikipedia.org/wiki/Entity%E2%80%93component%E2%80%93system).

Basically, every object in the game is a GameObject (aka entity), on which we can add or remove "Components" at runtime.
A Component can be anything, from sound effect, physics behavior, drawing a model, or scripting.

The GameObject is basically the 'node' in the scene, while the 'components' you attach to it are the interesting objects that interact and render stuff.
Most components will just use stuff from the 'Core' layer, and provide the API and the connection between independant 'Core' components and the 'GameObject'.