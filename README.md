# Desolation: The Backrooms

**Desolation: The Backrooms** is an Android Backrooms horror game focused on a playable Level 0 experience: yellow corridors, damp carpet, fluorescent lights, unsettling audio, simple survival pressure, and a mobile-friendly UI.

The active development path is now **Unity**. The older **Godot** prototype and texture archive are still kept in the repository as reference/legacy material, but the Unity project is the main build target.

## Current project state

The repository currently contains:

- `UnityProject/` — the active Unity Android project.
- `UnityProject/Assets/Scripts/FirstPlayableBatch.cs` — current playable gameplay batch.
- `UnityProject/Assets/Scripts/DesolationRuntime.cs` — earlier Unity menu/runtime prototype kept for reference while the Unity migration continues.
- `UnityProject/Assets/Scripts/MissingFeaturesTab.cs` — in-game status/checklist tab.
- `godot/` — legacy Godot prototype, generated materials, and uploaded texture archive.
- `Documentation/` — build notes, migration notes, UI prompts, texture inventory, and APK download help.
- `.github/workflows/unity-cloud-release.yml` — Unity Cloud Build trigger, APK downloader, and GitHub Release publisher.
- `.github/workflows/godot-android-build.yml` — legacy Godot Android build workflow.

## Active engine and format

The game is currently being migrated and developed as:

- Engine: **Unity**
- Main language: **C#**
- Target platform: **Android**
- Orientation: **Landscape**
- App name: **Desolation: The Backrooms**
- Android package: `com.desolation.thebackrooms`
- Current version: `0.1.0`

The old Godot/GDScript files are still present for reference, but the active direction is Unity/C#.

## Current gameplay

The latest gameplay batch includes:

- First-person movement.
- Touch-friendly mobile buttons.
- Keyboard support for testing.
- A generated Level 0-style environment with corridors, walls, pillars, floor, lights, fuses, breaker, exit, lockers, and an entity.
- Objective loop: collect fuses, find the key, power the breaker, and escape.
- Basic enemy pressure.
- Locker hiding.
- Health, sanity, and stamina HUD.
- Pause/resume/main menu flow.
- Multiple save slots and settings are in progress from the earlier Unity runtime work.
- Status/missing-features screen for tracking what is complete and what still needs work.

## Controls

Keyboard test controls:

```text
WASD / Arrow keys = move
Mouse = look
Left Shift = sprint
E = use nearby object
C / Left Ctrl = hide near locker
Esc / Back = pause or go back
```

Mobile controls:

```text
Left side = movement area
Right side = look area
USE = interact with nearby object
SPRINT = sprint while moving
HIDE = hide near a locker
Pause button = pause menu
```

Touch controls are still being improved and should be tested on a real Android device.

## Unity Cloud Build

The main Android APK pipeline is Unity Cloud Build plus GitHub Actions.

Workflow:

```text
.github/workflows/unity-cloud-release.yml
```

What it does:

1. Triggers the Unity Cloud Android build.
2. Waits for Unity to finish.
3. Downloads the APK artifact.
4. Publishes the APK to a GitHub Release.
5. Uploads logs if the Unity build or APK download fails.

Expected build time is often around **15–25 minutes**. That is normal for Unity Cloud Android builds.

Required repository secrets:

```text
UNITY_BUILD_API_KEY
UNITY_ARTIFACT_API_KEY
```

The workflow also supports the older fallback secret name:

```text
UNITY_BUILD
```

Do not commit Unity API keys or any other secrets into the repository.

## APK output

The Unity release workflow publishes an APK named:

```text
DesolationTheBackrooms-Unity.apk
```

The APK appears in GitHub Releases after a successful workflow run.

If a compressed `.7z` or `.zip` file is used later, Android cannot install it directly. Extract the archive first, then install the `.apk`.

## How to test on Android

1. Open the latest successful GitHub Release.
2. Download `DesolationTheBackrooms-Unity.apk`.
3. On Android, allow installation from your browser or file manager if required.
4. Install the APK.
5. Launch **Desolation: The Backrooms**.
6. Test the main menu, play button, movement, USE button, hiding, breaker/exit objective, pause menu, and save flow.

## What is implemented

- Unity project scaffold.
- Unity Android settings.
- Unity Cloud Build workflow.
- GitHub Release publishing workflow.
- Main menu direction and UI style.
- Saves/settings/feedback/credits prototypes.
- Playable first-person Level 0 batch.
- Objective loop foundation.
- Enemy pressure foundation.
- Hiding foundation.
- HUD foundation.
- Persistent settings foundation.
- In-game missing/status tab.
- Legacy Godot prototype and texture archive kept for reference.

## Still missing / next batches

The game is still a prototype. The next work should be done in small batches and tested after each build.

High-priority next items:

1. Stabilize the current Unity scripts and fix any compile/runtime errors from the latest gameplay batch.
2. Make the Unity menu, saves, settings, feedback, credits, and gameplay runtime use one clean active script path.
3. Improve mobile touch input so movement and look feel good on a phone.
4. Improve the objective chain with clearer prompts, better save-slot resume, and better puzzle feedback.
5. Improve enemy behavior with line of sight, hearing, hiding validation, chase states, and fair fail states.
6. Add more Level 0 rooms, dead ends, maintenance rooms, landmarks, doors, trim, vents, signs, and rare props.
7. Replace procedural placeholder audio with authored compressed sound assets.
8. Import final texture assets into the Unity project and apply them to materials.
9. Add real post-processing polish: vignette, film grain, color grading, exposure tuning, fog, and head bob.
10. Optimize APK size and Android performance.
11. Test on real Android devices for crashes, heat, frame rate, touch layout, installation, pause/resume, and save recovery.
12. Prepare final store material: app icon, screenshots, trailer, privacy text, release notes, and versioning.

## Legacy Godot project

The `godot/` folder is the previous prototype path. It contains:

- Godot scripts and scene files.
- Generated Backrooms materials.
- Uploaded texture archive.
- Godot Android export configuration.

It is useful as a reference for materials, gameplay ideas, and old build history, but the active direction is Unity.

## Documentation

Useful documents:

```text
Documentation/UNITY_CLOUD_BUILD.md
Documentation/UNITY_MIGRATION_STATUS.md
Documentation/APK_DOWNLOAD_HELP.md
Documentation/UPLOADED_TEXTURE_ARCHIVE_INVENTORY.md
Documentation/Level0_Game_Design.md
Documentation/UI_Prompt_Pack.md
MISSING_FEATURES.md
CONTROLS.md
BUILD_ANDROID.md
```

## Development rule

Work in small batches. Build and test after each batch. Unity Cloud builds are slow, so avoid triggering a full APK build for every tiny edit unless a device test is needed.
