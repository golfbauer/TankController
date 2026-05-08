# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

A Unity-based tank controller. Originally a master's thesis, now being revisited for cleanup and further development. The runtime simulates a tracked vehicle: hull rigidbody driven by per-wheel rigidbodies, custom inspectors for authoring tanks in the editor, projectile/armor collision system, and a HUD.

## Unity / tooling

- **Unity Editor: `6000.3.15f1`** (pinned in `ProjectSettings/ProjectVersion.txt`). Open the project with this exact version.
- Input is partly migrated: `AmmunitionManager` uses the new Input System package (`UnityEngine.InputSystem`); `KeyboardInputMovementManager` still uses the legacy `Input` class. Unity warns about this on load — non-blocking.
- No CLI build/test scripts; everything runs through the Unity Editor:
  - Build: *File → Build Profiles*.
  - Tests: *Window → General → Test Runner*. The only test assembly is `AmmunitionTests` (under `Assets/Controller/Tests/Ammunition/`).
- Scenes live in `Assets/Scenes/`. `Movement.unity` is the only scene.

## Architecture

The runtime is organized **feature-per-module**. Each module under `Assets/Controller/Scripts/` has its own runtime `.asmdef` plus a sibling `<Module>/Editor/` subfolder with a `<Module>.Editor.asmdef` (`includePlatforms: ["Editor"]`) for its custom inspectors. Standalone (non-Editor) builds compile because no runtime asmdef references `UnityEditor`.

Top-level layout under `Assets/Controller/`:
- `Scripts/` — all runtime + editor code, one folder per module.
- `Demo/` — demo prefabs and materials (`Panzer4/`, `ThesisDemo/`, `Test.prefab`). Authoring content, not tests.
- `Tests/Ammunition/` — the only test assembly.

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
- **`Turret/`** — `Turret` (base), `Gun`, plus `HorizontalRotation` / `VerticalRotation` rotation managers. Flat layout — runtime files at the top level, editor scripts under `Turret/Editor/`.
- **`Ammunition/`** — `AmmunitionManager` handles input, reload, fire. Ammo types are ScriptableObject-style assets (`AmmunitionType`) under `Assets/Controller/Demo/Panzer4/Projectiles/AmmoTypes/`. Projectiles spawn from `SpawnPoint` and live as their own prefabs.
- **`ImpactCollision/`** — `CollisionManager.HandleImpact()` resolves which `ArmorSection` was hit (or returns `defaultArmorSection`).
- **`PlayerCamera/`** — Third-person and scoped camera modes (`PlayerCamera/Controllers/`) plus a small HUD framework under `PlayerCamera/UI/`: `UIElement` → `UIGroup` composition, with concrete `AmmoImageUIElement` / `AmmoTextUIElement` reading from `AmmunitionManager`.
- **`Utils/`** — `GUIUtils` and the shared `TankComponentEditor` base class used by every custom inspector.

### Conventions used in the codebase

- Each module has a flat layout: runtime files at the top level, editor scripts under `<Module>/Editor/`. Several modules also keep a `Messages.cs` (string constants for inspector UI) and a `Utils.cs` (helpers) at the top level — when adding inspector text, follow the existing `Messages.cs` pattern in that module.
- Custom inspectors extend `TankComponentEditor` (in `Utils/Editor/`). New editors should do the same.
- Public fields on MonoBehaviours are used for serialization (rather than `[SerializeField] private`). The custom inspectors call `serializedObject.FindProperty("fieldName")` by string name, so renaming a public field requires updating the matching editor.
