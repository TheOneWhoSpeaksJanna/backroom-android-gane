# Backrooms Level Zero

**Backrooms Level Zero** is a playable first-person Android horror exploration game set only in Backrooms Level 0, also known as **The Lobby**.

The current version is a native Android build that can be compiled entirely through GitHub Actions, which is important because the project owner only has a phone and no PC. It uses generated code-driven art, procedural layout, synthesized audio, and mobile touch controls.

## Current gameplay

- First-person movement.
- Main menu with `ENTER LEVEL 0`, `SETTINGS`, and `EXIT`.
- Loading screen with flickering fluorescent progress bar.
- Left virtual joystick.
- Right-side swipe look.
- `USE` button for nearby red wall buttons.
- Sprint with stamina.
- Improved HUD with objective text, stamina, sanity, pause, use, and sprint controls.
- Pause menu that stays open until the player taps resume or another menu action.
- Settings screen for sensitivity, brightness, volume, frame cap, graphics scale, and control layout.
- Control layout screen for opacity and button size.
- Procedural Level 0 maze.
- Yellow first-person sleeves/hands based on the player reference.
- Black shadow monster based on the monster reference.
- Three red wall buttons to unlock the strange exit door.
- Damp-carpet footsteps, fluorescent hum, distant thuds, buzzing, and hunting audio.
- Fog, vignette, film grain, stains, ceiling glow, head bob, and wall details.

## Art direction

The game follows the uploaded references in `References/images/` and the uploaded prompt pack recorded in `Documentation/UI_Prompt_Pack.md`:

- endless yellow corridors
- plain pillars
- stained wallpaper
- damp carpet
- fluorescent ceiling panels
- yellow player/hazmat figures
- black shadow entity
- strange empty rooms, pits, and lone chairs
- dark translucent horror UI panels
- dirty yellow Android-friendly large touch targets

The longer direction documents are at:

```text
Documentation/Level0_Game_Design.md
Documentation/UI_Prompt_Pack.md
```

## Engine / renderer used

Unity and Godot are still the preferred future engines for a true high-end 3D version, but this repository currently uses:

- Native Android Java
- Hardware-accelerated Android Canvas ray-cast renderer
- Generated procedural geometry
- Generated procedural audio
- Android Gradle Plugin 8.7.3

This keeps the APK buildable on GitHub Actions without a desktop Unity installation.

## Android requirements

- Package name: `com.backrooms.levelzero`
- Game name: `Backrooms Level Zero`
- Orientation: landscape
- Minimum Android version: Android 8.0 / API 26
- Target SDK: 35

## APK

The workflow builds the APK at:

```text
/Builds/BackroomsLevelZero.apk
```

The APK is debug-signed unless release signing is configured separately.

## Install the APK

Download the APK from the repository or from the GitHub Actions artifact, then install it on Android. You may need to allow installation from unknown sources for your browser or file manager.

ADB install command:

```bash
adb install -r Builds/BackroomsLevelZero.apk
```

## Open the source project

Open the repository root in Android Studio. Android Studio should detect the Gradle Android project automatically.

The workflow also creates:

```text
/Source/BackroomsLevelZeroProject.zip
```

## Rebuild the APK

From the repository root:

```bash
gradle :app:assembleDebug
mkdir -p Builds
cp app/build/outputs/apk/debug/app-debug.apk Builds/BackroomsLevelZero.apk
```

The GitHub Actions workflow does this automatically on pushes to `main` and on manual workflow runs.

## Future high-end path

A true PC-comparable mobile version should eventually move to Unity 6 LTS with URP, baked fluorescent lighting, modular 3D assets, occlusion culling, texture compression, and a real 3D monster model. Until a Unity-capable build pipeline is available, this native Android version is the phone-friendly playable build.
