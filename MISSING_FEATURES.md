# Missing Features / Upgrade Plan

This file tracks what is still missing before the project feels like a high-end, realistic Backrooms Level 0 Android horror game.

## Current state

The project now has a playable Godot 4.4.1 APK build path, true 3D Level 0 foundation, generated Backrooms materials, and a fuller gameplay pass. It is still not a finished commercial game, but the prototype now includes a complete objective loop, menus, mobile controls, checkpointing, atmosphere, pressure mechanics, interaction prompts, basic hiding, procedural placeholder audio, and extra Level 0 landmarks.

## Implemented in first gameplay pass

- Main menu, pause menu, restart, quit, and settings panel.
- Settings sliders for look sensitivity, brightness, and volume.
- Objective loop: collect three fuses, power the service exit, and escape.
- Win and lose states with end screens.
- Interaction/proximity logic for fuses, notes, and the exit door.
- Mobile checkpoint save/load logic using `user://checkpoint.cfg`.
- Polished virtual joystick, look area, sprint button, pause button, stamina display, and sanity display.
- Simple psychological threat/entity that follows the player and drains sanity when close.
- Ambient event messages, carpet step feedback, flicker/hum/thud text cues, fog, vignette-like pressure shade, and brightness tuning.
- More Level 0 variety: pillars, grime patches, notes, maintenance/dead-end geometry, exit landmark, and extra material usage.

## Implemented directly on `main`

- Enabled `FeatureCompleter.gd` as a Godot autoload.
- Added manual USE/E interaction prompts for notes, locked service doors, hiding spots, and the exit.
- Added Android/back-key handling for gameplay, pause, settings, and title states.
- Added frame-cap and graphics-scale settings to the settings panel.
- Added hiding spots that reduce threat pressure and restore a small amount of sanity.
- Added a locked service door, another landmark route, and extra environmental notes.
- Added procedural placeholder audio for hum, footsteps, thuds, buzzing, and menu feedback.
- Fixed the objective-note typo from `BAUE` to `BLUE`.

## Still missing gameplay

- A deeper narrative route with multiple endings and more varied clue chains.
- More advanced enemy behavior, line-of-sight hiding, distraction, or stealth tools.
- Multiple checkpoints/save slots and resume-from-menu UX.
- More authored door/switch/locker puzzles beyond the current locked-door and note interactions.

## Still missing mobile features

- Full control layout customization.
- Touch-friendly settings persistence across app restarts.
- Low-data download page/asset instructions for the compressed `.apk.7z` file. Android cannot install `.sevenZ`/`.7z` directly; the APK must be extracted first.

## Still missing visual and audio polish

- More modular Level 0 sets: damaged wallpaper variants, maintenance rooms, dead ends, and rare landmarks.
- Replace procedural placeholder sounds with authored audio assets for final polish.
- Real post-processing pass for film grain, color grading, vignettes, and tuned head bob.
- More lighting variety with authored broken lights, dark pockets, and flicker zones.

## Still missing optimization

- Release export mode; the current pipeline uses debug export for reliability.
- Smaller texture budgets, audio compression, and unused asset removal.
- Occlusion and visibility optimization.
- Cheap lighting, baked/static lights where possible, and very limited real-time shadows.
- APK size pass: keep only required CPU architecture and minimize Android template bloat.

## Best next order

1. Replace procedural audio placeholders with authored compressed sound assets.
2. Add more modular room prefabs and authored landmarks.
3. Improve threat behavior with line-of-sight, hiding validation, and stealth logic.
4. Add control layout customization and persistent settings.
5. Add multiple save slots/resume UX.
6. Convert debug build into a smaller release build.
7. Continue APK size optimization.
