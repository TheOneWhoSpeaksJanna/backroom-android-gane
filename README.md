# Desolation: The Backrooms

**Desolation: The Backrooms** is an Android Backrooms horror game. The active project is the Unity project in `UnityProject/`. The older Godot folder is kept as a legacy prototype and as an asset/texture archive.

## Current format

- Engine: Unity
- Language: C#
* Target: Android
- App name: Desolation: The Backrooms
- Main Unity project: `UnityProject/`
- Legacy reference project: `godot/`

## Current gameplay state

The current Unity build has a playable Level 0 prototype with:

- Main menu, status screen, settings, pause, save, load, and resume flow.
- First-person movement.
- Health, sanity, and stamina.
- Mobile-style USE, SPRINT, HIDE, DECOY, and BAG buttons.
- Level 0 maze geometry with lights, pillars, partitions, carpet, wallpaper, ceiling, and metal props.
- Objective chain: notes, fuses, yellow card, blue card, valve, code panel, pressure gauge, switch order, breaker, exit, and truth door.
- More authored clue pacing through extra note/clue objects.
- Puzzle variety beyond the original fuses, keys, switches, breaker, and doors.
- Enemy patrol, investigate, search, chase, attack, and give-up behavior.
- Line-of-sight and hearing checks.
- Fair hiding validation near lockers.
- Multiple endings with clearer requirements.
- Inventory/item inspection through the BAG flow.
- Save-slot previews and resume-from-menu support.
- Customizable mobile control layout, touch-look tuning, and larger accessibility UI scaling.
- High culling/visibility optimization so distant or off-camera scene renderers are disabled instead of adding a low-quality mode.
- Android back-button handling.
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

This is the only missing-feature list for the project. It belongs in the README, not inside the game UI.

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
- Better material tuning for roughness, metalness, and normal maps.
- More optimized high-quality mobile lighting.
- App icon, splash screen, store screenshots, and trailer images.

### Missing optimization/release work

- Release export mode after the debug build path is stable.
- Texture compression and smaller texture budgets.
- Audio compression.
- Unused asset cleanup.
- Static/baked lighting where possible.
- Occlusion/visibility optimization pass.
- APK size pass.
- High-quality Android performance tuning without a low-quality mode.
- Final release notes and versioning.

### External QA still required

- Real-device testing for touch reliability, frame rate, heat, install, suspend, resume, and save recovery.
- Testing on your actual Android phone is still required because this repository work cannot physically run the APK on your device.

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
Q = throw distraction
Esc / Back = pause or go back
```

Mobile testing:

```text
Left side = movement area
Right side = look area
USE = interact with nearby object
SPRINT = sprint while moving
HIDE = hide near locker
DECOY = throw distraction
BAG = inventory / item inspection
Back button = pause or go back
```

## Development rule

Work in small batches, then test the Unity build. Unity Cloud builds are slow, so avoid triggering a full APK build for every tiny edit unless a device test is needed.
