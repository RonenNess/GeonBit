# (Alternative) Manual Setup

If you don't want to use *GeonBit*'s template (not recommended!), you can also install *GeonBit* manually:

### Setup GeonBit.UI

First setup GeonBit.UI, which is the UI system that comes with *GeonBit*.
Its git and installation instructions can be found [here](https://github.com/RonenNess/GeonBit.UI).

### Setup GeonBit Core

After the UI is setup, install the *GeonBit* NuGet package with the following command:

```
Install-Package GeonBit
```

This will also install the required dependencies.

Once the all the NuGet packages are successfully installed, follow these steps: 

1. Add all the Content that comes with the package (under the ```Content/GeonBit/``` folder).
2. Instead of implementing ```MonoGame``` 'Game' class, inherit and implement a ```GeonBit.GeonBitGame``` class, and implement the following functions:
	1. ```Draw()``` to add extra drawing logic.
	2. ```Update()``` to add extra update logic.
	3. ```Initialize()``` to create your scene and initialize stuff.
3. From your program's ```Main()``` function, Run *GeonBit* with your GeonBitGame class: ```GeonBitMain.Instance.Run(new MyGeonBitGame());```

