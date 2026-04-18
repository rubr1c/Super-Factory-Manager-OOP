# Super Factory Manager (OOP)

Unity factory-building game with a **grid-based**, **object-oriented** simulation: place machines, route **items**, **fluids**, and **energy** through pipes, run **recipes**, and manage **inventory** and **timelines**. The codebase is organized in layers (data, gameplay, application, presentation) so machines, items, and UI stay separated and testable.

## Features

- **Production simulation** — Recipe-driven machines (smelting, assembly, fabrication, power generation, extraction, logistics buffers, and uplinks).
- **Transport** — Dedicated pipe types for items, fluids, and energy.
- **Progression** — Upgrades, modifiers, and placeable entities on a grid.
- **UI** — Hotbar, inventory, entity info, auth, and timeline menus (Unity UI Toolkit: UXML/USS where referenced by the project).
- **Backend (optional)** — [Firebase](https://firebase.google.com/) (App + Auth) for sign-in and related services; Android/Google Play integration settings are present in project metadata.

## Requirements

| Tool | Notes |
|------|--------|
| **Unity Editor** | **6000.3.x** (this repo was last generated against **6000.3.4f1**). Install via [Unity Hub](https://unity.com/download). |
| **.NET** | Scripting backend and API compatibility follow Unity 6 defaults (see `Assembly-CSharp.csproj` / Editor Player Settings after opening the project). |
| **Android build** (optional) | Android SDK/NDK, OpenJDK, and Gradle setup as required by Unity’s Android module; Firebase and Play Services use the usual `google-services.json` / EDM4U flow. |

## Getting started

1. Clone the repository.
2. Open the project folder in **Unity Hub** and launch it with a **Unity 6 (6000.3.x)** editor.
3. Allow Unity to import assets and regenerate the `Library` folder (ignored by Git).
4. Open the main scene (e.g. `Assets/Scenes/` — often `SampleScene` or your bootstrap scene).
5. Press **Play** in the Editor to run.

If Unity prompts to upgrade the project when using a newer patch release, prefer staying on the **6000.3** line unless you intentionally migrate.

## Project layout (high level)

Typical Unity layout for this game:

- **`Assets/Scripts/`** — Game code, grouped by concern, for example:
  - **`Application/Managers`** — Simulation tick and timeline orchestration.
  - **`Core`** — Shared types (directions, interfaces, upgrade slots, modifiers).
  - **`Data`** — Items, recipes, catalogs, and databases.
  - **`Gameplay`** — Grid entities, machines, pipes, world/timeline logic.
  - **`Presentation`** — Camera, input, UI.
  - **`Systems`** — Inventory and containers.
- **`Assets/Firebase/`** & **`Assets/Plugins/`** — Firebase and native/plugin dependencies (managed with the External Dependency Manager where applicable).
- **`Assets/StreamingAssets/`** — Runtime-readable config (e.g. Firebase desktop helper JSON if used).
- **`ProjectSettings/`** — Unity and platform settings (including Google Play Services–related entries where present).

Exact folder names match what Unity and Rider/Visual Studio generate from your `Assets` tree.

## Firebase & secrets

- Firebase is wired for **Unity** with config files such as `google-services.json` and related assets under `StreamingAssets` (see your `Assembly-CSharp.csproj` **None** includes for the exact list).
- **Do not commit** private keys, real production `google-services.json` contents, or service accounts if this repo is public. Use placeholders or separate private config for CI and team machines.

## Building

- **Desktop**: use **File → Build Settings**, choose PC/Mac/Linux, then **Build**.
- **Android**: switch platform to **Android**, configure **Player Settings** (package name, signing), resolve dependencies (Firebase/EDM4U), then **Build** or **Build And Run**.

## Development notes

- Rendering uses the **Universal Render Pipeline (URP)** (URP global settings present at the project root).
- Input uses the **new Input System** (`InputSystem_Actions.inputactions` at project root).
- Open **`Super Factory Manager OOP.sln`** in JetBrains Rider or Visual Studio for C# editing; generated `.csproj` files are Unity-managed.
