# Setup

There are 3 ways to setup *GeonBit*, listed below:

## From Template

*GeonBit* requires some external libs, specific init code, and built-in content.
Because of that, the best and most simple way to setup *GeonBit* is via the ```Visual Studio``` templates.

To do so, simply install the template file attached to this git ([found here](https://github.com/RonenNess/GeonBit/Template)) and create a new projects from that template. Your project should compile and run out of the box.

## From Empty Project

If you don't like to use templates for whatever reason, you can download a working empty *GeonBit* project from [this repo](https://github.com/RonenNess/GeonBit.Template).

Just clone the git and change project settings and name to fit your needs.

## Manual Setup

If you choose to setup *GeonBit* manually, for whatever reason, please follow these steps:

### Setup GeonBit.UI

First setup GeonBit.UI, which is the UI system that comes with *GeonBit*.
Its git and installation instructions can be found [here](https://github.com/RonenNess/GeonBit.UI).

### Setup GeonBit Core

After the UI is setup, install the *GeonBit* NuGet package with the following command:

```
Install-Package GeonBit.Core
```

This will also install the required dependencies.

Once the all the NuGet packages are successfully installed, follow these steps: 

1. Add all the Content that comes with the package (under the ```Content/GeonBit/``` folder).
2. Instead of implementing ```MonoGame``` 'Game' class, inherit and implement a ```GeonBit.GeonBitGame``` class, and implement the following functions:
    1. ```Draw()``` to add extra drawing logic.
    2. ```Update()``` to add extra update logic.
    3. ```Initialize()``` to create your scene and initialize stuff.
3. From your program's ```Main()``` function, Run *GeonBit* with your GeonBitGame class: ```GeonBitMain.Instance.Run(new MyGeonBitGame());```

