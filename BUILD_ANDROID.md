# Android Build Instructions

## Exact project toolchain

- Language: Java
- Renderer: Native Android OpenGL ES 2.0
- Build system: Gradle
- Android Gradle Plugin: `8.7.3`
- Gradle version used by CI: `8.9`
- Java version used by CI: Temurin JDK 17
- Android compile SDK: `35`
- Android target SDK: `35`
- Android minimum SDK: `26` / Android 8.0
- Android build tools installed by CI: `35.0.0`
- Package name: `com.backrooms.levelzero`
- Orientation: Landscape
- Build output expected by workflow: `/Builds/BackroomsLevelZero.apk`

## GitHub Actions build

The repository includes:

```text
.github/workflows/android-build.yml
```

It performs these steps:

1. Checks out the project.
2. Installs JDK 17.
3. Installs Android command-line tooling.
4. Installs Android SDK Platform 35 and Build Tools 35.0.0.
5. Uses Gradle 8.9.
6. Runs:

   ```bash
   gradle :app:assembleDebug
   ```

7. Copies the generated APK to:

   ```text
   Builds/BackroomsLevelZero.apk
   ```

8. Creates:

   ```text
   Source/BackroomsLevelZeroProject.zip
   ```

9. Uploads both files as GitHub Actions artifacts.
10. Commits the generated `Builds` and `Source` files back to `main` when possible.

## Local build steps

### 1. Install prerequisites

Install:

- Android Studio, or Android command-line tools
- JDK 17
- Gradle 8.9
- Android SDK Platform 35
- Android SDK Build Tools 35.0.0
- Platform Tools if you want to install with ADB

### 2. Open the project

Open the repository root in Android Studio.

### 3. Sync Gradle

Let Android Studio sync the project. The project uses:

```gradle
id 'com.android.application' version '8.7.3'
```

### 4. Build the debug APK

From the repository root:

```bash
gradle :app:assembleDebug
```

### 5. Copy the APK to the requested output path

```bash
mkdir -p Builds
cp app/build/outputs/apk/debug/app-debug.apk Builds/BackroomsLevelZero.apk
```

### 6. Install on device

```bash
adb install -r Builds/BackroomsLevelZero.apk
```

## Release build

A release build task exists:

```bash
gradle :app:assembleRelease
```

However, the default project does not include a private signing key. To create a distributable release APK, add your own keystore through Android Studio or CI secrets. Do not commit signing keys to the repository.

## Validation checklist

After building and installing:

1. Launch **Backrooms Level Zero**.
2. Confirm the first scene opens into Level 0.
3. Use the left joystick to walk.
4. Swipe on the right side to look around.
5. Hold Sprint to move faster.
6. Open the pause button in the top-right corner.
7. Adjust sensitivity and graphics quality.
8. Confirm fluorescent hum and ambience are audible.
9. Walk through the procedural rooms and find the strange exit door.
10. Confirm lighting, fog/haze, vignette, and generated materials are visible.

## Troubleshooting

### Gradle command not found

Install Gradle 8.9 or use Android Studio's Gradle integration.

### Android SDK missing

Install SDK Platform 35 and Build Tools 35.0.0 through Android Studio SDK Manager or:

```bash
sdkmanager "platforms;android-35" "build-tools;35.0.0" "platform-tools"
```

### App installs but shows a black screen

Make sure the test device supports OpenGL ES 2.0. Most Android 8.0+ devices do.

### No audio

Check device volume and media volume. The game uses generated `AudioTrack` audio and does not require external audio assets.

### Release APK not signed

Debug builds are automatically debug-signed. Release builds require your own keystore. Never commit keystores or passwords.
