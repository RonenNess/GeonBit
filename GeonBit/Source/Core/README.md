# Core

'Core' contains all the basic stuff used to implement different Components. You can think of this as the "driver" layer.
For example, it contains basic nodes and entities to draw stuff, or basic physics and collision entities. Then the "Component" layer uses this Core for its internal implementation:

GameObject:
	Component --> Core
	Component --> Core
	...

For example, lets talk about drawing a model. First we have the GameObject entity. Then we attach to it a DrawModel component. But the component itself doesn't know how to draw stuff, so it uses a "GraphicModel" entity from the Core layer.
That's the basic role of the Core.