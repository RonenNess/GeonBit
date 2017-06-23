# Plugins

Its very common to define your own Component types and custom Managers in GeonBit, and even more common to want to reuse or share them with other people.
The best way to do it is to create Plugins.

You can create custom a plugin by making a dll that reference ```GeonBit.Core``` and implements a special class called ```GeonBitPluginInitializer```, which will be described shortly.
Inside your custom plugin you can define any number of Components, Managers, and other cool Utils, and adding plugins to GeonBit is simply a matter of referencing the plugin's dll.

## Core Plugins

The following is a list of GeonBit core plugins that ship separately and you can easily add to your GeonBit projects:

- LuaScript: plugin that add support in dynamically-loaded Lua scripts (via MoonSharp). TBD
- CSharpScript: plugin that add support in dynamically-loaded C# scripts (via Roslyn). TBD


## GeonBitPluginInitializer

To make your plugin recognized by *GeonBit*, define anywhere in your assembly a public class called ```GeonBitPluginInitializer```, that implements the following *public* static functions:

#### GetName()

Should return your plugin's name.

#### Initialize()

Called when your plugin is loaded by GeonBit.

### Example

Here's an example of a basic ```GeonBitPluginInitializer``` class and how it should look like:

```cs
public static class GeonBitPluginInitializer
{
    // get plugin name
    static public string GetName() 
    {
        return "MyPlugin";
    }
    
    // initialize plugin
    static public void Initialize()
    {
        // do init code
    }
}
```

Or, you can fetch a *GeonBit* plugin template from [here](https://github.com/RonenNess/GeonBit/PluginTemplate).

Note that you can define a dll with custom Components etc without making that special class and it will still be usable, but in this case GeonBit won't know this plugin is loaded and you will have to initialize your Managers and other stuff manually.
Its also a good practice to make your dlls a proper GeonBit plugins for whatever future features GeonBit might add for plugins.

## Creating Plugin Components

To create a custom component, simply inherit from the ```GeonBit.ECS.Components.BaseComponent``` class and implement the required functions.
Its recommended to put all your plugin Component classes under ```GeonBit.ECS.Components.<PluginName>``` namespace, to avoid collision with other plugins or future core features.

Your custom components may access any core components, utilities and other built-ins GeonBit provides.

## Creating Plugin Managers

To create a custom manager, simply inherit from the ```GeonBit.Managers.IManager``` interface and implement the required functions.
Note that all managers must be registered into GeonBitMain prior Initialization, using the following API:

```cs
// managerInstance is an instance of your manager class.
GeonBit.GeonBitMain.RegisterManager(managerInstance);
```

To do so please use the ```GeonBitPluginInitializer``` Initialize() function.

