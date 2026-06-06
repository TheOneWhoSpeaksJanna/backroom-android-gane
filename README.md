# Desolation: The Backrooms

**Desolation: The Backrooms** is an Android Backrooms horror game. The active project is now the Unity project in `UnityProject/`. The older Godot folder is kept as a legacy prototype and as an asset/texture archive.

## Current format

- Engine: Unity
- Language: C#
- Target: Android
- App name: Desolation: The Backrooms
- Main Unity project: `UnityProject/`
- Legacy reference project: `godot/`

## Current gameplay state

The current Unity build has a playable Level 0 prototype with:

- Main menu, status screen, settings, pause, save, and load.
- First-person movement.
- Health, sanity, and stamina.
- Mobile-style USE, SPRINT, HIDE, and DECOY buttons.
- Level 0 maze geometry with lights, pillars, partitions, carpet, wallpaper, ceiling, and metal props.
- Objective chain: collect fuses, find keys, activate switches, power the breaker, and escape.
- Notes and a truth-door ending route.
- Locker hiding.
- Enemy pressure and chase behavior.
- Decoy distraction.
- Android back button handling.
- Unity texture bridge that uses the texture archive from the old Godot folders.

## Texture bridge

You do not need to manually move the uploaded Godot textures.

The Unity editor/build bridge copies textures from:

```text
godot/assets/textures/uploaded/
godot/assets/textures/
```

into:

```text
UnityProject/Assets/Resources/Textures/
```

during Unity import/build. Runtime materials load them through:

```text
Resources.Load<Texture2D>("Textures/<texture_name>")
```

Important texture names currently used by the Unity runtime include:

```text
wall_leak_color
wall_leak_alt_c_color
carpet_fabric_color
office_ceiling_color
painted_metal_color
office_ceiling_emission
plastic_panel_color
```

## Missing tab

This is the only missing-feature tab/list for the project. It belongs in the README, not inside the game UI.

### Missing gameplay

- Better authored objective pacing and clue chains.
- More puzzle variety beyond fuses, keys, switches, breaker, and doors.
- More advanced enemy states: patrol, investigate, search, chase, attack, give up, and fair hiding validation.
- Better line-of-sight and hearing logic.
- More endings with clearer narrative requirements.
- Inventory/item inspection.
- More checkpoint and save-slot polish.

### Missing mobile features

- Full customizable control layout.
- Better touch-look tuning on real Android hardware.
- Larger accessibility scaling options.
- Save-slot previews/screenshots.
- More complete resume-from-menu UX.
- Real-device testing for touch reliability, frame rate, heat, install, suspend, resume, and save recovery.

### Missing level/content polish

- More modular Level 0 room pieces.
- More dead ends, maintenance spaces, vents, landmarks, signs, trim, doors, stains, outlets, and rare props.
- Better placement of the uploaded textures across different surface types.
- More lighting pockets, dark areas, flicker zones, and broken lights.

### Missing audio polish

- Final authored fluorescent hums.
- Carpet footsteps.
- Entity cues.
- Distant thuds and buzzes.
- Jumpscare stingers.
- UI click/confirm/back sounds.
- Audio mixer groups and Android-friendly compression.

### Missing visual polish

- Post-processing: vignette, film grain, exposure, color grading, bloom, fog tuning, and head bob.
- Better material tuning for roughness/metalness/normal maps.
- More optimized mobile lighting.
- App icon, splash screen, store screenshots, and trailer images.

### Missing optimization/release work

- Release export mode after the debug build path is stable.
- Texture compression and smaller texture budgets.
- Audio compression.
- Unused asset cleanup.
- Static/baked lighting where possible.
- Occlusion/visibility optimization.
- APK size pass.
- Low-end Android quality mode.
- Final release notes and versioning.

## Unity Cloud Build

The main Android build workflow is:

```text
.github/workflows/unity-cloud-release.yml
```

Expected build time is often around 15–25 minutes. That is normal for Unity Cloud Android builds.

The workflow:

1. Triggers Unity Cloud Build.
2. Waits for the Android build.
3. Downloads the APK artifact.
4. Publishes the APK to GitHub Releases.
5. Uploads logs if the Unity build or artifact download fails.

## APK output

The expected Unity APK artifact/release asset is:

```text
DesolationTheBackrooms-Unity.apk
```

Android cannot install `.7z` or compressed archive files directly. If a compressed archive is ever used later, extract the APK first, then install the APK.

## Controls

Keyboard testing:

```text
WASD / Arrow keys = move
Mouse = look
Left Shift = sprint
E = use
C / Left Ctrl = hide near locker
Q = throw decoy
Esc / Back = pause or go back
```

Mobile testing:

```text
USE = interact with nearby object
SPRINT = sprint while moving
HIDE = hide near locker
DECOY = throw distraction
Back button = pause or go back
```

## Development rule

Work in small batches, then test the Unity build. Unity Cloud builds are slow, so avoid triggering a full APK build for every tiny edit unless a device test is needed.
