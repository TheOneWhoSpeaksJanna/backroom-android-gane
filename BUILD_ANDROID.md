# Android Build Instructions

## Real app format

This project is a Godot Android game named **Desolation: The Backrooms**.

```text
Engine: Godot 4.4.1
Game code: GDScript
Scenes/UI: .tscn
Assets: JPG/PNG under godot/assets/
Android output: APK exported by Godot
```

The old native Java/Gradle Android placeholder project was removed because it was not the real game and produced the tiny placeholder APK.

## Use this workflow

Use only this workflow for game APK builds:

```text
Build and Release Desolation Android APK
.github/workflows/godot-android-build.yml
```

This workflow builds the real Godot project under:

```text
godot/
```

## What it creates

Every successful run creates:

```text
Builds/DesolationTheBackrooms.apk
Builds/DesolationTheBackrooms.apk.7z
Source/DesolationTheBackroomsProject.zip
```

It uploads those files as workflow artifacts and publishes them to a GitHub Release.

## How to run the build

Go to:

```text
Actions -> Build and Release Desolation Android APK -> Run workflow
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
2. Download the artifact named `DesolationTheBackrooms-build-files`, or
3. Open the latest GitHub Release and download:
   - `DesolationTheBackrooms.apk`
   - `DesolationTheBackrooms.apk.7z`
   - `DesolationTheBackroomsProject.zip`

## Safety check

The workflow fails if the APK is under 1 MB. That prevents tiny placeholder APKs from being released accidentally.
