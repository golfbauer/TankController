# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

A Unity-based tank controller. Originally a master's thesis, now being revisited for cleanup and further development. The runtime simulates a tracked vehicle: hull rigidbody driven by per-wheel rigidbodies, custom inspectors for authoring tanks in the editor, projectile/armor collision system, and a HUD.

## Unity / tooling

- **Unity Editor: `2021.3.22f1`** (pinned in `ProjectSettings/ProjectVersion.txt`). Open the project with this exact version.
- Input handled via the new Input System package (`UnityEngine.InputSystem`).
- No CLI build/test scripts; everything runs through the Unity Editor:
  - Build: *File → Build Settings*.
  - Tests: *Window → General → Test Runner*. The only test assembly is `AmmunitionTests` (under `Assets/Controller/Scripts/Ammunition/Tests/`).
- Scenes live in `Assets/Scenes/`. `Movement.unity` is the main demo scene. `TriangleProblem.unity` is a leftover scratch scene.

## Architecture

The runtime is organized **feature-per-module**, with one assembly definition (`.asmdef`) per module. All modules live under `Assets/Controller/Scripts/`.

### Module dependency graph

`Tank` is the composition root and references every other module:

```
Tank → Movement, Wheels, Turret, ImpactCollision, PlayerCamera, Utils
Movement → (independent, owns its own input layer)
Wheels   → Utils
Turret   → Utils
ImpactCollision → Ammunition (armor responds to projectile hits)
Ammunition → Utils
PlayerCamera → Ammunition (HUD reads ammo state)
Utils → (leaf)
```

When adding cross-module references, update the module's `.asmdef` `references` array — implicit references won't compile.

### Module roles

- **`Tank/`** — Settings holder (`Tank.cs`) plus the custom inspector that authors a tank prefab. Toggles which managers (`useCameraManager`, `useCollisionManager`, `useMovementManager`) get attached at author time. Not a runtime coordinator.
- **`Movement/`** — `MovementManager` reads from a swappable `MovementInputManager` (currently `KeyboardInputMovementManager`) and produces left/right track speeds, torques, and drag. `Wheels` reads those values via `WheelGroupManager`.
- **`Wheels/`** — Per-wheel `Rigidbody` model. Subfolders per wheel type (`DriveWheel`, `RearWheel`, `SupportWheel`, `SuspensionWheel`, `Chain`). `WheelManager` applies torque per `FixedUpdate`; `WheelGroupManager` is the bridge from `Movement`. Note: this is a physically expensive design (10–20 rigidbodies per tank) — be aware before adding more wheels.
- **`Turret/`** — `Turret` (base), `Gun`, plus `HorizontalRotation` / `VerticalRotation` managers. Folder is laid out as `Editors/` and `Managers/`, but `Editors/Base/Turret.cs` is **runtime** code despite the folder name. Don't be misled.
- **`Ammunition/`** — `AmmunitionManager` handles input, reload, fire. Ammo types are ScriptableObject-style assets (`AmmunitionType`) under `Assets/Controller/Tests/Test_Prefabs/Panzer 4/Projectiles/AmmoTypes/`. Projectiles spawn from `SpawnPoint` and live as their own prefabs.
- **`ImpactCollision/`** — `CollisionManager.HandleImpact()` resolves which `ArmorSection` was hit (or returns `defaultArmorSection`).
- **`PlayerCamera/`** — Third-person and scoped camera modes plus a small HUD framework: `UIElement` → `UIGroup` composition, with concrete `AmmoImageUIElement` / `AmmoTextUIElement` reading from `AmmunitionManager`. Note the folder name collision: `PlayerCamera/Movement/` is camera movement, not tank movement.
- **`Utils/`** — `GUIUtils` and the shared `TankComponentEditor` base class used by every custom inspector.

### Conventions used in the codebase

- Most modules have a `Services/` subfolder containing a `Messages.cs` (string constants for inspector UI) and a `Utils.cs` (helpers). When adding inspector text, follow the existing `Messages.cs` pattern in that module.
- Custom inspectors extend `TankComponentEditor` (in `Utils/`). New editors should do the same.
- Public fields on MonoBehaviours are used for serialization (rather than `[SerializeField] private`). The custom inspectors call `serializedObject.FindProperty("fieldName")` by string name, so renaming a public field requires updating the matching editor.

## Known structural issues (read before adding code)

- **Editor scripts live in runtime assemblies without `#if UNITY_EDITOR` guards.** Every `*Editor.cs` sits next to its runtime sibling and the parent `.asmdef` has no editor-platform split. Standalone (non-editor) builds will currently fail to compile. When adding new editor code, either wrap it in `#if UNITY_EDITOR ... #endif` or place it in a future `Editor/` subfolder. Don't make this worse.
- The branch `improve-structure` is ~8.9k lines ahead of `master` with informal commits ("stuff"); current work continues there. `master` is stale.
- `TriangleProblem/` (scripts + scene) is an empty experiment and is unrelated to the tank.
