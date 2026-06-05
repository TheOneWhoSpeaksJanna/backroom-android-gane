# Android Build Instructions

## Use this workflow

Use only this workflow for game APK builds:

```text
Build and Release Godot Android APK
.github/workflows/godot-android-build.yml
```

This is the only workflow that should build the real Godot game APK and publish release files.

## What it creates

Every successful run creates:

```text
Builds/BackroomsLevelZero.apk
Builds/BackroomsLevelZero.apk.7z
Source/BackroomsLevelZeroProject.zip
```

It also uploads those files as workflow artifacts and publishes them to a GitHub Release.

## Why the old APK was only about 24 KB

The old workflow named `Build Android APK` built the native Android placeholder project under `app/`, not the Godot project under `godot/`.

That APK is tiny because it is not the full Godot game. It is now disabled so it does not confuse releases.

## How to run the build

Go to:

```text
Actions -> Build and Release Godot Android APK -> Run workflow
```

Or push a change to one of these paths:

```text
godot/**
Documentation/**
MISSING_FEATURES.md
README_GODOT_3D.md
.github/workflows/godot-android-build.yml
```

## Where to download the APK

After the workflow succeeds:

1. Open the completed workflow run.
2. Download the artifact named `BackroomsLevelZero-build-files`, or
3. Open the latest GitHub Release and download:
   - `BackroomsLevelZero.apk`
   - `BackroomsLevelZero.apk.7z`
   - `BackroomsLevelZeroProject.zip`

## Safety check

The workflow fails if the APK is under 1 MB. That prevents another tiny placeholder APK from being released accidentally.

## Disabled workflows

These old workflows are intentionally disabled and only exist so old links do not disappear:

```text
android-build.yml
apply-critical-fixes.yml
apply-uploaded-textures.yml
finish-ui-pause-stamina.yml
patch-backrooms-ui-controls-logs.yml
patch-collision-ui.yml
patch-gameplay-ui.yml
run-backrooms-ui-patch.yml
```

Do not use those for releases.
