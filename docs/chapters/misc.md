# Misc

## Working on GeonBit

This repo contains the source of GeonBit engine. It should build to a cross-platform Any CPU dll, and should not run directly from here.

### Distributing New Version

To distribute a new version follow these steps:

- Build the new dll.
- Update NuGet package - be sure to update version in both the nuget spec and in the code const.
- Build a new template (described later).
- Put the result template under ```Template/``` folder.

### Updating GeonBit.UI

When updating GeonBit.UI version, we need to update the version via the NuGet package manager, but after installation remove all of GeonBit.UI content and resources.
The reason for that is that we don't want to store them in the git (while they already appear in GeonBit.UI git) and when we build the template (will be described soon) those resources will be restored anyway.

### Building The Template

To distribute GeonBit we build a visual studio template.

To build a new template, go to the [GeonBit.Template](https://github.com/RonenNess/GeonBit.Template) project (note: its private) and update GeonBit to latest version.

Follow the instructions in git, and put the result in ```Template/``` folder.


## TBDS

Things I want to add to GeonBit in near future.

- GPU particles system.
- LoD renderer - a component that is a collection of renderer components and switch between them automatically based on distance from camera.
- Physical material.

## Credits

- Kastellanos Nikolaos for Skinned Model animations (https://github.com/tainicom/Aether.Extras).
