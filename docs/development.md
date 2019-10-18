---
title: Development
mainNavigation: true
order: 30
---

# Development

The Kentico Inspector solution consists of both the backend and frontend projects. The backend and C# frontend target [.NET Core 2.2](https://dotnet.microsoft.com/download/dotnet-core/2.2) while the Vue client in the frontend targets [Vue 2.6.8](https://vuejs.org/).

## Philosophy

The Kentico Inspector solution follows a simple philosophy:

- Be consistent
- Be readable
- Be minimal
- Be self-documented

## Preparing the environment

The following steps should get you started with developing Kentico Inspector:

1. Install [Visual Studio 2017 ~15.9.11](https://visualstudio.microsoft.com/vs/).
1. (Recommended) Install [Visual Studio Code](https://code.visualstudio.com/).
1. Install [.NET Core 2.2 SDK](https://dotnet.microsoft.com/download/dotnet-core/2.2).
1. Install [Node ~10.15.3](https://nodejs.org/en/).
1. Open `.\KenticoInspector.WebApplication\ClientApp` in Visual Studio Code or a terminal.
1. Run `npm i`.
1. Run `npm run build`.
1. If you are not planning on developing the Vue client, you can close Visual Studio Code or the terminal.
1. Open `.\KInspector.sln` in Visual Studio.
1. Build the solution.
1. Ensure `Debug` is the selected configuration and `KenticoInspector.WebApplication` is the startup project.
1. Select either `IIS Express` or `Console` as the debug target.
1. Start a debug session (F5) to launch Kentico Inspector.

If you have switched branches, it is a good idea to clear your browser cache when Kentico Inspector launches.
