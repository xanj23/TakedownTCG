# Takedown TCG

## Status

This repository contains two separate applications:

* `TakedownTCGApplication`: ASP.NET Core MVC web application
* `TakedownTCG`: Console CLI application

## Project Scope

Takedown TCG is a C# trading card search application for exploring card, set, game, listing, and completed sales data through supported external APIs.

The web application and CLI are maintained as separate programs within the same repository.

## Features

* Search for cards using a required text query with optional card number, printing, and condition filters.
* Search for sets by game with optional text search and sorting options.
* Retrieve the full `/games` endpoint from the application menu.
* Build URL-safe query strings from user input.
* Deserialize JustTCG JSON responses into strongly typed C# models.
* Search using JustTCG, Pokémon TCG, eBay, and SerpApi-powered completed sales data.
* Support account, favorites, and search workflows.
* Run the web application and CLI independently.

## Current Architecture

* `TakedownTCG.slnx`
  Root solution containing the web application, CLI application, and test projects.

* `src/TakedownTCGApplication/`
  ASP.NET Core MVC application containing controllers, Razor views, view models, application services, infrastructure, and static assets.

* `src/TakedownTCGCli/`
  Console application containing menu controllers, CLI views, the JustTCG client flow, user account services, and persistence.

* `tests/TakedownTCG.Core.Tests/`
  Service and behavior tests for account, favorite, and search workflows.

* `tests/TakedownTCG.Web.Tests/`
  Endpoint tests for the ASP.NET Core MVC application.

## Setup

1. Install the .NET 10 SDK.

2. From the repository root, build the solution:

```powershell
dotnet build .\TakedownTCG.slnx
```

3. Run the ASP.NET Core MVC application:

```powershell
dotnet run --project .\src\TakedownTCGApplication\TakedownTCGApplication.csproj
```

4. Run the CLI application:

```powershell
dotnet run --project .\src\TakedownTCGCli\TakedownTCG.csproj
```

5. Run the test projects:

```powershell
dotnet test .\TakedownTCG.slnx
```

## Documentation

* `README.md`
  High-level project scope, architecture, and setup information.

* `TakedownTCG.slnx`
  Main solution entry point for build and test workflows.

## Developers

* Xan Johnson
* Chris Herriman Jr

