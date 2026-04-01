# OOP-TCG-App

## Status
- This repository contains the legacy CLI version.
- `TakedownTCG` will be the official CLI version going forward.
- An application/GUI will be added in a future release.

## Project Scope
OOP-TCG-App is a C# console application for exploring trading card game data through the JustTCG API. The current implementation focuses on object-oriented modeling of API responses and interactive search workflows for cards, games, and sets.

## Features
- Search cards by required text query with optional number, printing, and condition filters.
- Search sets by game with optional text search and sort options.
- Retrieve the full `/games` endpoint from the menu.
- Build URL-safe query strings from user input.
- Deserialize JustTCG JSON responses into strongly typed C# models.
- Read the JustTCG API key from the `JUSTTCG_API_KEY` environment variable instead of storing credentials in source control.
- Available APIs: 1 (`JustTCG`).

## Current Architecture
- `OOP-TCG-APP/Program.cs`
  Starts the console app and initializes API authentication.
- `OOP-TCG-APP/Services/`
  Contains the menu flow, input collection classes, request building, and helper utilities.
- `OOP-TCG-APP/Models/API Models/`
  Contains object-oriented models for the API response payloads.
- `OOP-TCG-APP/Models/Query Models/`
  Contains query parameter models and enums used by the input services.
- `OOP-TCG-APP/Search.cs`
  Contains a typed JustTCG client prototype for card searching with paging and result mapping.

## Setup
1. Install the .NET SDK used by the project.
2. Set the `JUSTTCG_API_KEY` environment variable.
3. From the repository root, run:

```powershell
dotnet build .\OOP-TCG-APP\OOP-TCG-APP.csproj
dotnet run --project .\OOP-TCG-APP\OOP-TCG-APP.csproj
```

## Documentation
- `README.md`
  High-level project scope and setup information.
- `SearchDocumentation.txt`
  Notes about the implemented search flow, query parameters, and related classes.

## Planned Direction
The broader project idea still points toward richer price tracking, watchlists, and additional user-facing features, but the repository currently implements the console search foundation and the supporting models needed for future expansion.
