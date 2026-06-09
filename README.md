# Desolation: The Backrooms — Android

A first-person horror game set in the Backrooms, built in Unity for Android.

## Project Structure

```
backroom-android-gane/
├── .github/
│   └── workflows/
│       └── unity-cloud-release.yml    # CI: triggers Unity Cloud Build + GitHub Release
├── UnityProject/
│   ├── Assets/
│   │   ├── AppIcon.png                # App icon
│   │   ├── Editor/                    # Editor-only scripts (build processing, branding)
│   │   │   ├── DesolationAndroidBranding.cs
│   │   │   ├── DesolationBuildSetup.cs
│   │   │   └── TextureBridgeBuildProcessor.cs
│   │   ├── Resources/
│   │   │   └── Textures/              # Runtime-loaded textures (carpet, walls, ceilings, etc.)
│   │   ├── Scenes/
│   │   │   ├── DesolationBootstrap.unity
│   │   │   └── MainMenu.unity
│   │   ├── Scripts/
│   │   │   ├── Data/
│   │   │   │   └── UnitySaveManager.cs    # Save/load game state (GameStateData)
│   │   │   ├── DesolationRuntime.cs        # Main menu system (OnGUI) — 5 screens
│   │   │   ├── FirstPlayableBatch.cs      # First-person Backrooms gameplay
│   │   │   ├── GameplayMobileCompletion.cs
│   │   │   ├── MissingFeaturesTab.cs
│   │   │   ├── Environment/
│   │   │   │   └── LightFlicker.cs        # Flickering light effect
│   │   │   ├── Inventory/
│   │   │   │   └── InventoryManager.cs    # Inventory system
│   │   │   ├── Items/
│   │   │   │   ├── AlmondWater.cs         # Consumable item
│   │   │   │   └── BatteryAction.cs       # Battery interaction item
│   │   │   └── Player/
│   │   │       ├── PlayerMovement.cs      # First-person movement controller
│   │   │       └── SanityManager.cs       # Sanity meter system
│   │   └── UI/                            # Design reference screenshots
│   │       ├── 01_main_menu_clean.png
│   │       ├── 02_save_slots_clean.png
│   │       ├── 03_settings_clean.png
│   │       ├── 04_credits_clean.png
│   │       └── 05_feedback_clean.png
│   ├── Packages/
│   │   └── manifest.json
│   └── ProjectSettings/
│       ├── EditorBuildSettings.asset
│       ├── ProjectSettings.asset
│       └── ProjectVersion.txt
├── .gitignore
└── README.md
```

## Build Pipeline

Every push to `main` automatically triggers a Unity Cloud Build:

1. Unity Cloud Build compiles the Android APK
2. The APK is downloaded and compressed into a ZIP
3. A GitHub Release is created with both the APK and ZIP

Workflow: `.github/workflows/unity-cloud-release.yml`

Required GitHub Secrets:
- `UNITY_BUILD_API_KEY` — Unity Cloud Build API key
- `UNITY_ARTIFACT_API_KEY` — Unity Artifact API key (fallback)

## Menu System

The menu is drawn entirely via Unity's `OnGUI()` (IMGUI) system in `DesolationRuntime.cs`.
It renders 5 screens matching the industrial backrooms aesthetic:

| Screen | Description |
|--------|-------------|
| **Main** | Hero title "DESOLATION: THE BACKROOMS" with Play, Settings, Credits buttons |
| **Saves** | 3 save slots with unlock/lock state, level names, golden card design |
| **Settings** | Volume sliders (master/music/SFX), brightness, graphics quality capsules |
| **Credits** | Scrollable credits listing team members and contributors |
| **Feedback** | Text input + email + category dropdown with send button |

Visual style: Dark panels with golden-orange glow, scanline overlay, vignette effect,
pulsing title text, glitch effects — matching the liminal backrooms horror aesthetic.

## How to Contribute

1. Clone the repo
2. Open `UnityProject/` in Unity (check `ProjectVersion.txt` for the correct editor version)
3. Make changes, commit, and push to `main`
4. The Unity Cloud Build will trigger automatically
5. Check the GitHub Actions tab for build status and the Releases tab for the APK

## Design Reference

The `UnityProject/Assets/UI/` directory contains 5 clean design screenshots showing the
target visual style for each menu screen. These are the reference images for the IMGUI
menu rendering in `DesolationRuntime.cs`.
