# Let's go through that README again... What the heck is this?
A C# NetCore open source server for that other leg-bone game which must not be named, apparently, because it may upset people. 

Here's a nice pic (from early dev), because pics are good:

   ![Image of developing Fibula](images/fibulaDev.PNG?raw=true)

# TL;DR What are you looking for?

Run this thing? see [how to setup and run it](articles/setup.md).

Knobs and switches? [configuration](articles/configuration.md) section.

Just... [WHY?](articles/motivation.md).

Status and overview of things implemented? Err... [overview / roadmap](articles/roadmap.md)... sure.

###### More docs to (probably) come soon!

# How is this better than the existing C++ engines?
Well, it's too soon to say, but it sure looks sexy. The main development features right now are:

## Top-of-industry standards + clean code = happy devs.
It really grinds my gears how badly TFS is documented overall, and how steep the learning curve for newbies is. 
Ergo, I strived to make this is a well-documented and clean code project that can actually be maintained.

   ![Image of no warnings.](images/hashtagnowarnings.PNG?raw=true)

Check the [code reference here](code/index.md), which pulls the XML documentation from the actual code to generate these pages. It doesn't get better than that.

## We got them tests.
Slowly growing in the repo, the Fibula project also features testing made super easy by the dotnet core framework.

   ![Image of some test projects.](images/testProjects.PNG?raw=true)

   ![Image of some test run.](images/someTestRun.PNG?raw=true)

## Dependency Injection:
Dependency injection gives the power to the dev, to quickly switch out the flavor of specific component that they want to run, e.g. the way the map loads:

   ![Image of more dependency injection.](images/dependencyInjection.PNG?raw=true)

> Notice the OTBM flavor in the image above.

And the same can be done for other assets loading.

## Minimal changes between backend version.
By leveraging DI we can support different back-end versions with minimal code changes to make when switching between them.

Take 7.72 for example:

   ![Image of 7.72 project.](images/multiVersion.PNG?raw=true)
   
This project (and thus, DLL) contains all the intrinsics of packet parsing and writing, connection and other components specific to that version:

   ![Image of a packet reader.](images/perVersionPacketEx.PNG?raw=true)

And it is injected with 2 lines at the composition root of the project `(bottom 2 lines)`:

   ![Image of the composition root.](images/compositionRoot.PNG?raw=true)

Once we want to support another version, say `7.4`, we shall add a single DLL targetting that version, implementing the components needed to be injected, possibly point to the `7.4` map/assets in the config, and re-compile.

> For the above example: `7.4` did not have XTEA or RSA encryption for connections, so that would be stripped.
> 
> Moreover, for `7.1` for example, the structure would change to _not include `skulls` or `shields`_ in the player status packet.
