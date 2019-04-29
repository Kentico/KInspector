# Kentico Inspector

[![Join the chat at https://kentico-community.slack.com](https://img.shields.io/badge/join-slack-E6186D.svg)](https://kentico-community.slack.com)
[![Build status](https://ci.appveyor.com/api/projects/status/udykjx510v83w9y6?svg=true)](https://ci.appveyor.com/project/kentico/kinspector)
[![first-timers-only](https://img.shields.io/badge/first--timers--only-friendly-blue.svg)](http://www.firsttimersonly.com/)
[![Github All Releases](https://img.shields.io/github/downloads/kentico/kinspector/total.svg)](https://github.com/Kentico/KInspector/releases)

Kentico Inspector (formerly KInspector) is an application for analyzing the health, performance and security of **[Kentico EMS](https://www.kentico.com/)** solutions.

Kentico Inspector was initially developed as an internal application by the Kentico consulting team to help evaluation customer's web sites. We quickly realized that the broader community would benefit from this as well, so we made it open source.

The application is Kentico version agnostic and has no dependencies on version-specific DLLs. Most modules are designed to support version 10 and later, but some will work on older versions as well.

## Get the application

Download the [latest release](https://github.com/Kentico/KInspector/releases/latest). **ADD MORE DETAILS FOR V4**

## Contributing

Want to improve the Kentico Inspector? Great! Read the [contributing guidelines](https://github.com/Kentico/KInspector/blob/master/CONTRIBUTING.md) and then [check out the open issues](https://github.com/Kentico/KInspector/issues) (especially issues marked as "[good first issue](https://github.com/Kentico/KInspector/labels/good%20first%20issue)") to get started.

If anything feels wrong or incomplete, please let us know. Create a new [issue](https://github.com/Kentico/KInspector/issues/new) or submit a [pull request](https://help.github.com/articles/using-pull-requests/).

## Development Requirements

All versions below are from a known working environment. Lower versions may work but are not tested.

- Visual Studio 2017 with latest update (15.9.11+)
- .NET Core 2.2 SDK
- Node for Windows (10.15.X+)
- NPM (6.4.X+)

### First run

To build the Client UI application (required anytime the client UI code is updated unless you are using the `UI Development` debug launch setting):

1. Open Powershell
1. Change the directory to `./KenticoInspector.WebApplication/ClientApp`
1. Run `npm i`
1. Run `npm build`

To build the Backend:

1. First, build the Client UI
1. Open `KInspector.sln` in Visual Studio
1. Do a build
1. Make sure the `KenticoInspector.WebApplication` proejct is the start up project
1. You can run it with either the `IIS Express` or `Console` debug launch settings

If you want to work on the Client UI applicaiton, there's a few additional steps to go through.

1. Open the `./KenticoInspector.WebApplication/ClientApp` directory in your editor of choice (Visual Studio Code is recommended) as well as in Powershell
1. In Powershell, run `npm i` (if you haven't yet) and `npm run serve`
1. Leave the application is running.
1. Follow the steps to build the backend, but run it using the `UI Development` debug launch settings.
   - This runs the backend with a proxy to the running instance you started in Powershell and allows you to take advantage of the hot-reloading of the clietn application

![Analytics](https://kentico-ga-beacon.azurewebsites.net/api/UA-69014260-4/Kentico/KInspector?pixel)
