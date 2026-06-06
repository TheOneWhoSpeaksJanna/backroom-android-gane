# Desolation: The Backrooms

**Desolation: The Backrooms** is a high-quality Android Backrooms horror game built in Unity.

## Current format

- Engine: Unity 6 LTS
- Language: C#
- Target: Android
- App name: Desolation: The Backrooms
- Main Unity project: `UnityProject/`
- UI Kit: Desolation Backrooms UI Kit (Canvas-based menu system)

## Current gameplay state

The Unity build has a playable Level 0 prototype with:

- Main menu, settings, pause, save, load, slot previews, and resume flow.
- First-person movement with Android-style touch controls.
- Health, sanity, stamina, sprinting, hiding, decoys, and item inspection.
- Objective chain with notes, fuses, yellow card, blue card, valve, code panel, pressure gauge, switch order, breaker, exit, and truth door.
- Extra authored clue chains and clearer ending requirements.
- Enemy patrol, investigate, search, chase, attack, and give-up behavior.
- Line-of-sight, hearing, entity cue, and fair locker hiding validation.
- Modular Level 0 room pieces, dead ends, maintenance spaces, vents, signs, trim, outlets, stains, doors, rare props, and extra landmarks.
- Improved uploaded texture placement across wall, carpet, ceiling, metal, plastic/sign, stain, prop, and lighting surfaces.
- Lighting pockets, dark zones, flicker lights, broken fluorescent panels, fog tuning, vignette-style overlays, film grain, and head bob.
- Authored runtime audio polish: fluorescent hum, carpet footsteps, entity cues, distant thuds/buzzes, jumpscare stingers, and UI click feedback.
- High culling/visibility optimization so off-camera or hidden scene renderers are disabled instead of adding a low-quality mode.
- Android-focused controls: touch look tuning, accessibility scale, audio volume, customizable layout support, and Android back-button handling.
- App icon configured from the uploaded golden **D** Backrooms phone image at `UnityProject/Assets/AppIcon.png`.

## App icon and branding

The uploaded phone image is now the app icon source:

```text
UnityProject/Assets/AppIcon.png
UnityProject/Assets/Editor/DesolationAndroidBranding.cs
```

`DesolationAndroidBranding.cs` applies the icon to Android Player Settings before build and sets Android-friendly audio import compression for authored audio clips.

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

## Remaining release work

The remaining items are release/QA tasks rather than core missing gameplay:

- Real-device Android testing for touch reliability, frame rate, heat, install, suspend, resume, and save recovery.
- Final manual review of APK size and device performance.
- Store-ready screenshots/trailer export after the final APK build is stable.
- Final release notes and versioning.

## Unity Cloud Build

The main Android build workflow is:

```text
.github/workflows/unity-cloud-release.yml
```

Expected build time is often around 15–25 minutes for Unity Cloud Android builds.

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
