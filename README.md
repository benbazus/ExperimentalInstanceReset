# Reset Experimental Instance

<!-- Replace this badge with your own-->
[![Build status](https://ci.appveyor.com/api/projects/status/hv6uyc059rqbc6fj?svg=true)](https://ci.appveyor.com/project/madskristensen/extensibilitytools)

<!-- Update the VS Gallery link after you upload the VSIX-->
Download this extension from the [VS Gallery](https://visualstudiogallery.msdn.microsoft.com/[GuidFromGallery])
or get the [CI build](http://vsixgallery.com/extension/a452f910-6b6e-4ebd-952f-d8f1cd8c09b4/).

---------------------------------------

Reset Visual Studio Experimental Instance.

See the [changelog](CHANGELOG.md) for changes and roadmap.

## Features

- Reset and Clean Visual Studio Experimental Instance

### Reset Exp Instance
In Visual Studio's top menu under Tools, a new command is now visible:


![Context Menu](art/context-menu.png)


Clicking the **Reset Experimental Instance** button will prompt you to confirm
the resetting process.

![Prompt](art/Prompt.png)


The progress of the resetting process. When it finishes, the progress form will close.

![Progress](art/progress.png)


## Contribute
Check out the [contribution guidelines](CONTRIBUTING.md)
if you want to contribute to this project.


For cloning and building this project yourself, make sure
to install the
[Extensibility Tools 2015](https://visualstudiogallery.msdn.microsoft.com/ab39a092-1343-46e2-b0f1-6a3f91155aa6)
extension for Visual Studio which enables some features
used by this project.


## License
[Apache 2.0](LICENSE)