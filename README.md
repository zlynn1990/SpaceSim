# SpaceSim

SpaceSim is an n-body simulation of the solar system complete with stars, planets, moons, and spacecraft. It is currently setup to simulate various SpaceX launch profiles with vehicles including the Dragon, Falcon 9, and Falcon Heavy.

## Installation

If you just want to run the simulation, download the latest build found under the builds directory. Unzip the build and launch SpaceSim.exe to get started. Otherwise, you can clone this repository and browse the source code.

In order to have the best graphically experience, your video card must support double precision openCL kernels. If support isn't detected the program will fallback to basic GDI rendering.

The program accepts two command line arguments. If you want to run the game in windowed mode add a "-w". To launch a custom flight profile use the profile's name like "Falcon Heavy 35000kg".

## Controls

The control scheme is similar to Kerbal Space Program.

| Button | Action |
| ------ | ----------- |
| Enter  | Starts the simulation. |
| Escape | Exits the simulation. |
| Scroll | Changes the zoom factor. |
| [ | Switch focus to previous body. |
| ] | Switch focus to next body. |
| , | Reduce simulation speed. |
| . | Increase simulation speed. |

## Flight Profiles

Flight profiles are found in the 'flight profiles' root directory. To add a new flight profile create a folder under that directory with a unique name. Flight profiles consist of various xml files with controls for the vehicles components. The control format allows for 5 basic commands (ignition, shutdown, throttle, stage, and orient). The payload.xml file specifies the payload properties like DryMass and PropellantMass. Use the included flight profile as a guide for creating new profiles.

See the instructions above for launching custom profiles.