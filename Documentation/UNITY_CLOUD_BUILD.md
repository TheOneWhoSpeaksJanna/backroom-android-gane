# Unity Cloud Build Setup

Unity Build Automation should point at this subfolder:

```text
UnityProject
```

Recommended configuration:

```text
Target name: Desolation Android
Branch: main
Project subfolder path: UnityProject
Unity version: 6000.0.58f2
Android SDK: 35
Platform: Android
Bundle ID: com.desolation.thebackrooms
Build format: APK
Machine: Micro
Auto-build: Off
```

The Unity runtime in `Assets/Scripts/DesolationRuntime.cs` now includes:

- title menu
- three save slots
- settings
- APK help
- generated Level 0 rooms
- fuse objectives
- notes
- hiding spots
- breaker and locked door puzzle
- scrap distraction
- entity pressure
- sanity drain
- multiple endings

Use the manual Build button after Unity finishes indexing the repo. If the build fails, copy the first red error lines from the Unity Cloud build log.

The Godot version remains in `godot/` until the Unity version is tested and accepted on device.
