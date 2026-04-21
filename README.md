# OOP-TCG-App

## Status
- This repository contains two separate applications: an ASP.NET Core MVC web app and a console CLI.
- `TakedownTCGApplication` is the MVC web application.
- `TakedownTCG` is the CLI application.

## Project Scope
OOP-TCG-App is a C# trading card search application for exploring card, set, game, listing, and completed-sales data through supported external APIs. The current implementation keeps the web application and CLI as separate programs under the same repository.

## Features
- Search cards by required text query with optional number, printing, and condition filters.
- Search sets by game with optional text search and sort options.
- Retrieve the full `/games` endpoint from the menu.
- Build URL-safe query strings from user input.
- Deserialize JustTCG JSON responses into strongly typed C# models.
- Run from checked-in configuration without presentation-time environment variable setup.
- Available search APIs include JustTCG, Pokemon TCG, eBay, and SerpApi-powered completed sales.

## Current Architecture
- `TakedownTCG.slnx`
  Root solution containing the web app, CLI app, and test projects.
- `src/TakedownTCGApplication/`
  ASP.NET Core MVC application with controllers, Razor views, view models, application services, infrastructure, and static assets.
- `src/TakedownTCGCli/`
  Console application with menu controllers, CLI views, JustTCG client flow, user account services, and persistence.
- `tests/TakedownTCG.Core.Tests/`
  Service and behavior tests for account, favorite, and search workflows.
- `tests/TakedownTCG.Web.Tests/`
  Web endpoint tests for the MVC application.

## Setup
1. Install the .NET SDK used by the project.
2. From the repository root, run:

```powershell
dotnet build .\TakedownTCG.slnx
dotnet run --project .\src\TakedownTCGApplication\TakedownTCGApplication.csproj
dotnet run --project .\src\TakedownTCGCli\TakedownTCG.csproj
```

## Documentation
- `README.md`
  High-level project scope and setup information.
- `TakedownTCG.slnx`
  The main solution entry point for build and test workflows.

## Planned Direction
The broader project direction points toward richer price tracking, watchlists, and additional user-facing features while keeping the web and CLI applications independently runnable.
