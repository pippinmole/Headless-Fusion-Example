# Headless Fusion Sample

Headless Fusion Sample is a Unity sample package for dealing with headless server instances for the [Photon Fusion](https://doc.photonengine.com/en-us/fusion/current/getting-started/fusion-intro) networking package.

This repository uses some code from the official Photon Fusion Headless Server sample, which you can find [here](https://doc.photonengine.com/en-us/fusion/current/samples/fusion-headless), and extends this with a terminal to product output.

## Installation

1. Pull the repo
2. Create an application ID for Fusion on the [Photon Dashboard](https://dashboard.photonengine.com/)
3. Open in Unity
4. Install the [latest version](https://doc.photonengine.com/en-us/fusion/current/getting-started/sdk-download) of Fusion and import into the project
5. Navigate to `Fusion > Fusion Hub` and set up Fusion by pasting in your application ID

## Usage

To run a server instance:
1. Open the `Bootstrap` scene, enable the `Server Bootstrap` gameobject and disable the `Client Bootstrap` gameobject
2. Navigate to `File > Build Settings` and add `Bootstrap` scene to the `Scenes in Build` field
3. Click `Build` and select a destination

When running the server build, you will need a batch file in order to pass parameters to the application. This is a template script you can use to get the bare minimum working.

```bash
@echo off

"./Headless Fusion Example.exe" -batchmode -nographics

pause
```

You can make use of these arguments in the application through the static `HeadlessUtils` class, like so:
```csharp
var scene = HeadlessUtils.GetArg("-scene");
```

## Issues
If you run into any issues setting up the project, please feel free to open up an issue.

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[MIT](https://choosealicense.com/licenses/mit/)