# Contributing Setup

## Requirements

All versions below are from a known working environment. Lower versions may work but are not tested.

- [Visual Studio 2017 updated to 15.9.11 or later](https://visualstudio.microsoft.com/vs/)
- [.NET Core 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node for Windows (10.15.X - 20.7.0)](https://nodejs.org/en/)
- [NPM (6.4.X - 10.1.0) (included with Node)](https://www.npmjs.com/)

## Running the application

1. Open Powershell/Command Prompt
1. Change the directory to `src/KInspector.Blazor`
1. Run `npm i`
1. Open `src/KInspector.sln` in Visual Studio
1. Make sure the `KenticoInspector.Blazor` project is the start up project
1. You can run it with either the `IIS Express` or `Console` debug launch settings

Whenever changes are made to `wwwroot/css/app.css`, or new classes are added to the Razor files, you must regenerate the minified CSS file:

```bash
npm run css
```