---
title: Installation
mainNavigation: true
---

# Installation

Kentico Inspector supports two modes: **Console mode** and **IIS mode**. Start by downloading the [latest release](https://github.com/Kentico/KInspector/releases/latest).

## Console mode

1. Extract package contents in any folder.
2. Run `KenticoInspector.WebApplication.exe`.
3. Browse to [https://localhost:5001](https://localhost:5001) or [http://localhost:5000](http://localhost:5000).

## IIS mode

1. Extract package contents in any folder.
2. Point an IIS site or application to the folder.
3. Ensure the application pool's .NET CLR version is set to **No managed code**.
4. Browse to the IIS site.
