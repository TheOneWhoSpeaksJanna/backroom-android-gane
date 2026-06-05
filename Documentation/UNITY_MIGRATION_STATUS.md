# Unity Migration Status

The Unity version is now more than a launch test scaffold.

Updated Unity runtime:

```text
UnityProject/Assets/Scripts/DesolationRuntime.cs
```

Current Unity features:

- Desolation: The Backrooms title screen.
- Slot based new game/continue menu.
- Settings for sensitivity, volume, and touch button scale.
- APK download help screen for `.apk` and `.apk.7z` files.
- Generated Level 0 rooms, carpet, ceiling, walls, pillars, and warm lights.
- First-person player controller.
- Mobile look and virtual button controls.
- Three fuses, five notes, breaker, locked door, exit door, and scrap throw.
- Entity pressure chase system with sanity drain.
- Save/load progress via Unity PlayerPrefs.
- Multiple endings: escape, truth, lost, and desolation.

Next Cloud Build step:

```text
Unity Cloud -> Build Automation -> Desolation Android -> Build
```

If the build fails, open the failed build log and share the first red error lines.
